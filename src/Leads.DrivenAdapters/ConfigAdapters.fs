module Leads.DrivenAdapters.ConfigAdapters

open System
open System.IO
open System.Threading.Tasks
open Leads.Core.Config.Models
open Leads.Core.Config.Workflows
open Leads.Core.Utilities.Result
open YamlDotNet.Serialization


let yamlFileConfigurationProvider: ConfigurationProvider =
    fun _ ->
        Task.FromResult(Ok None)
        // result {
        //     let filePath = $"{Environment.SpecialFolder.UserProfile}/config.yaml"
        //     match File.Exists(filePath) with
        //     | false ->
        //         return None                
        //     | true ->
        //         try
        //             let yamlContent = File.ReadAllText filePath            
        //             let deserializer = DeserializerBuilder().Build()          
        //             return Some(deserializer.Deserialize<Configuration>(yamlContent))
        //         with
        //             | excp ->
        //                 let c = Error(excp.Message)
        //                 return c
        // }
                  
        