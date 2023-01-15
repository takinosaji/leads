module Leads.DrivenAdapters.ConfigAdapters

open System
open System.IO
open Leads.Core.Config.Workflows
open Leads.Core.Utilities.Result
open YamlDotNet.Serialization


let yamlFileConfigurationProvider: ConfigurationProvider =
    fun _ ->
        result {
            let filePath = $"{Environment.SpecialFolder.UserProfile}/leads-config.yaml"
            match File.Exists(filePath) with
            | false ->
                return None                
            | true ->
                try
                    let yamlContent = File.ReadAllText filePath            
                    let deserializer = DeserializerBuilder().Build()          
                    return Some(deserializer.Deserialize<Configuration>(yamlContent))
                with
                    | excp ->
                        let c = 
                        return c
        }
                  
        