module Leads.DrivenAdapters.ConfigAdapters

open System
open System.IO
open YamlDotNet.Serialization

open Leads.Core.Config.Workflows
open Leads.Core.Utilities.ConstrainedTypes

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
         
                   
         