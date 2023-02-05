module Leads.DrivenAdapters.ConfigAdapters

open System.IO
open FSharp.Json

open Leads.Core.Config
open Leads.Core.Utilities.ConstrainedTypes

// TODO: write unit tests
let private provideJsonFileConfiguration =
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
                        Error(ErrorText excp.Message)
            )

// TODO: write unit tests
let private applyJsonFileConfiguration =
    fun filePath key value ->
        let keyString = ConfigKey.value key
        let valueString = ConfigValue.value value
                
        let newConfigSource = provideJsonFileConfiguration filePath
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
                Error(ErrorText excp.Message)       
        | Error errorText -> Error errorText
      
let createLocalJsonFileConfigAdapters configFilePath =
    {|
       provideJsonFileConfiguration = fun (_:unit) -> provideJsonFileConfiguration configFilePath
       applyJsonFileConfiguration = fun key value -> applyJsonFileConfiguration configFilePath key value
    |}
            
