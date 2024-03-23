module Leads.SecondaryAdapters.SQLiteBased.ConfigAdapters

open System.Data.SQLite
open System.IO
open Leads.SecondaryPorts.Config.DTO

module AllowedKeys =
    let DefaultForestKey = "default.forest"
    let WorkingDirKey = "working.dir"
open AllowedKeys

let internal getConnectionString
    (defaultWorkingDirPath: string)
    (validConfigurationDto: ValidConfigSIDto) =
    let workingDirPath = ValidConfigSIDto.findOrDefault validConfigurationDto WorkingDirKey defaultWorkingDirPath
    let dataSource = Path.Combine(workingDirPath, "leads.db")
    $"Data Source={dataSource};Version=3;"

let private initializeDatabase defaultWorkingDirPath validConfigurationDto =
    let connectionString = getConnectionString defaultWorkingDirPath validConfigurationDto
    use connection = new SQLiteConnection(connectionString)
    connection.Open()
    use command = new SQLiteCommand(connection)
    command.CommandText <- """
        CREATE TABLE IF NOT EXISTS Configuration (
            Key TEXT PRIMARY KEY,
            Value TEXT NOT NULL
        );
    """
    command.ExecuteNonQuery() |> ignore

let private provideConfiguration defaultWorkingDirPath =
    fun validConfigurationDto ->
        initializeDatabase defaultWorkingDirPath validConfigurationDto
        let connectionString = getConnectionString defaultWorkingDirPath validConfigurationDto
        use connection = new SQLiteConnection(connectionString)
        connection.Open()
        use command = new SQLiteCommand(connection)
        command.CommandText <- "SELECT * FROM Configuration;"
        use reader = command.ExecuteReader()
        let configMap =
            [|
                while reader.Read() do
                    yield reader.GetString(0), reader.GetString(1)
            |]
            |> Map.ofArray
        Ok(Some configMap)

let private applyConfigValue defaultWorkingDirPath =
    fun validConfigurationDto keyString valueString ->
        initializeDatabase defaultWorkingDirPath validConfigurationDto
        let connectionString = getConnectionString defaultWorkingDirPath validConfigurationDto
        use connection = new SQLiteConnection(connectionString)
        connection.Open()
        use command = new SQLiteCommand(connection)
        command.CommandText <- """
            INSERT OR REPLACE INTO Configuration (Key, Value)
            VALUES (@key, @value);
        """
        command.Parameters.AddWithValue("@key", keyString) |> ignore
        command.Parameters.AddWithValue("@value", valueString) |> ignore
        command.ExecuteNonQuery() |> ignore
        Ok ()

let createLocalSQLiteConfigAdapters defaultWorkingDirPath =
    {|
       provideConfiguration = provideConfiguration defaultWorkingDirPath
       applyConfigValue = applyConfigValue defaultWorkingDirPath
       provideAllowedKeys = fun _ -> [DefaultForestKey; WorkingDirKey]
    |}