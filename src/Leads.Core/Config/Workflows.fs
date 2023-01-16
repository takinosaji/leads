module Leads.Core.Config.Workflows

open Leads.Core.Utilities.ConstrainedTypes
open Leads.Core.Utilities.Result
open Leads.Core.Utilities.Dependencies
open Leads.Core.Config.Models

type ConfigurationProvider = unit -> Result<Option<StringMap>, ErrorText>  
type ConfigEnvironment = {
    configProvider: ConfigurationProvider
}


type SetConfigWorkflow = string -> Reader<ConfigEnvironment, Result<unit, ErrorText>>


type GetConfigWorkflow = string -> Reader<ConfigEnvironment, Result<Option<ConfigValue>, ErrorText>>
let getConfigWorkflow: GetConfigWorkflow =
    fun requestedKey -> reader {
        let! services = Reader.ask
       
        return result {
            let! key = ConfigKey.create requestedKey            
            let! configurationText = services.configProvider()
            
            match configurationText with
            | Some stringMap ->
                let parsedConfiguration = Configuration.create stringMap
                let! value = Configuration.getValue key parsedConfiguration
                return value // TODO: why cant use return! - dig into extension code
            | None ->
                return None  
        }     
    }
        