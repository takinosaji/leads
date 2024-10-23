module Leads.SecondaryAdapters.SQLiteBased.MigrationRunner

open System.Reflection

open FluentMigrator.Runner
open FluentMigrator.Runner.Announcers
open FluentMigrator.Runner.Generators.SQLite
open FluentMigrator.Runner.Initialization
open FluentMigrator.Runner.Processors
open FluentMigrator.Runner.Processors.SQLite   



let ensureLeadsDatabaseIsUpToDate workingDirPath =
    try
        let connectionString = sprintf "Data Source=%s/leads.db" workingDirPath
        let options = ProcessorOptions()
        let assembly = Assembly.GetExecutingAssembly()
        let announcer = new TextWriterAnnouncer(fun msg -> ())
        let migrationContext = new RunnerContext(announcer)
        let factory = SQLiteDbFactory()
        let connection = factory.CreateConnection(connectionString)
        let quoter = SQLiteQuoter()
        let processor = new SQLiteProcessor(connection, SQLiteGenerator(quoter), announcer, options, factory, quoter)

        let runner = MigrationRunner(assembly, migrationContext, processor)
        runner.MigrateUp() 
       
        Ok()
    with excp ->
        Error excp.Message
