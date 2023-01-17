module Leads.DrivenAdapters.ConfigAdapters

open System
open System.IO
open Leads.Core.Config
open YamlDotNet.Serialization

open Leads.Core.Config.Workflows
open Leads.Core.Utilities.ConstrainedTypes

// TODO: write unit tests
let yamlFileConfigurationProvider: ConfigurationProvider =
    fun _ ->
        let filePath = $"{Environment.SpecialFolder.UserProfile}/leads-config.yaml"
        match File.Exists(filePath) with
        | false ->
            Ok None                
        | true ->
            try
                let yamlContent = File.ReadAllText filePath            
                let deserializer = DeserializerBuilder().Build()          
                Ok(Some(deserializer.Deserialize<Map<string,string>>(yamlContent)))
            with excp -> Error(ErrorText excp.Message)

// TODO: write unit tests
let yamlFileConfigurationApplier: ConfigurationApplier =
    fun key value ->
        let keyString = ConfigKey.value key
        let valueString = ConfigValue.value value
        
        let filePath = $"{Environment.GetFolderPath Environment.SpecialFolder.UserProfile}/leads-config.yaml"
        try
            use fileStream = File.Open(filePath, FileMode.OpenOrCreate)
            use reader = new StreamReader(fileStream)
            
            let content = reader.ReadToEnd()
            let deserializer = DeserializerBuilder().Build()
     
            let deserialized = deserializer.Deserialize(content, typedefof<Map<string,string>>)
            
            let newConfigSource =
                match deserialized with
                | :? Map<string,string> as map ->
                    match map.ContainsKey keyString with
                    | true -> map.Change(keyString, (fun _ -> Some valueString))
                    | false -> map.Add(keyString, valueString)
                | _ -> Map.empty.Add(keyString, valueString)
            
            let serializer = SerializerBuilder().Build()
            let yaml = serializer.Serialize(newConfigSource);
        
            File.WriteAllText(filePath, yaml)
            Ok ()
        with excp -> Error(ErrorText excp.Message)       
        
