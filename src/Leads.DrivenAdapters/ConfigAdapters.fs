module Leads.DrivenAdapters.ConfigAdapters

open System
open System.IO
open FSharp.Json
open Leads.Core.Config

open Leads.Core.Config.Workflows
open Leads.Core.Utilities.ConstrainedTypes

let filePath = $"{Environment.GetFolderPath Environment.SpecialFolder.UserProfile}/leads-config.yaml"


// TODO: write unit tests
let provideJsonFileConfiguration: ConfigurationProvider =
    fun _ ->
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
let applyJsonFileConfiguration: ConfigurationValueApplier =
    fun key value ->
        let keyString = ConfigKey.value key
        let valueString = ConfigValue.value value
                
        let newConfigSource = provideJsonFileConfiguration()
        match newConfigSource with        
        | Ok someSource ->   
            let source = match someSource with
                | None -> Map.empty.Add(keyString, valueString)
                | Some source -> match source.ContainsKey keyString with
                    | true -> source.Change(keyString, (fun _ -> Some valueString))
                    | false -> source.Add(keyString, valueString)
            try   
                let json = Json.serialize source    
                File.WriteAllText(filePath, json)
                Ok ()
            with excp ->
                Error(ErrorText excp.Message)       
        | Error errorText -> Error errorText
      
        
