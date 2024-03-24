module Leads.SecondaryAdapters.JsonFileBased.ConfigAdapters

open System.IO
open FSharp.Json

open Leads.SecondaryPorts.Config.DTO

module AllowedKeys = 
    let DefaultForestKey = "default.forest"
    let WorkingDirKey = "working.dir"
open AllowedKeys

let internal getWorkingDirPath
    (appDirPath: string)
    (validConfigurationDto: ValidConfigSIDto) = 
    ValidConfigSIDto.findOrDefault
        validConfigurationDto
        WorkingDirKey
        appDirPath

// TODO: write unit tests
let private provideConfiguration =
    fun filePath ->
        using (File.Open(filePath, FileMode.OpenOrCreate))
            (fun fileStream -> 
                use reader = new StreamReader(fileStream)
                let content = reader.ReadToEnd()
                match content with
                | "" -> Ok None
                | _ ->
                    try
                        Ok(Some(Json.deserialize<Map<string,string>> content))
                    with excp ->
                        Error(excp.Message)
            )

// TODO: write unit tests
let private applyConfigValue =
    fun filePath keyString valueString ->                
        let newConfigSource = provideConfiguration filePath
        match newConfigSource with        
        | Ok someSource ->   
            let source =
                match someSource with
                | None -> Map.empty.Add(keyString, valueString)
                | Some source ->
                    match source.ContainsKey keyString with
                    | true -> source.Change(keyString, (fun _ -> Some valueString))
                    | false -> source.Add(keyString, valueString)
            try   
                let json = Json.serialize source    
                File.WriteAllText(filePath, json)
                Ok ()
            with excp ->
                Error(excp.Message)       
        | Error errorText -> Error errorText

let createLocalJsonFileConfigAdapters appDirPath =
    let configFilePath = $"{appDirPath}/config.json"
    
    {|
       provideConfiguration = fun () -> provideConfiguration configFilePath
       applyConfigValue = fun key value -> applyConfigValue configFilePath key value
       provideAllowedKeys = fun () -> [DefaultForestKey; WorkingDirKey]  
    |}
            
