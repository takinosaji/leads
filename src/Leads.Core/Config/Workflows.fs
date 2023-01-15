module Leads.Core.Config.Workflows

open Leads.Core.Utilities.Result
open Leads.Core.Utilities.Dependencies
open Leads.Core.Models
open Leads.Core.Config.Models

type ConfigurationProvider = unit -> Result<Option<StringMap>, ErrorText>  
type ConfigEnvironment = {
    configProvider: ConfigurationProvider
}

type GetConfigWorkflow = string -> Reader<ConfigEnvironment, Result<Option<ConfigValue>, ErrorText>>
type SetConfigWorkflow = string -> Reader<ConfigEnvironment, Result<unit, ErrorText>>



let getConfigWorkflow: GetConfigWorkflow =
    fun keyString -> reader {
        let! services = Reader.ask
       
        return result {
            let! (ConfigKey key) = configKeyFactory keyString
            let! config = services.configProvider()
            match config with
            | Some stringMap ->
                let! config = configFactory stringMap
                return Some config[key]
            | None ->
                return None
        }     
    }
        