module Leads.Core.Config.Workflows

open Leads.Core.Utilities.ConstrainedTypes
open Leads.Core.Utilities.Result
open Leads.Core.Utilities.Dependencies
open Leads.Core.Config.Models

type ConfigurationProvider = unit -> Result<ConfigurationSource, ErrorText>  
type ConfigEnvironment = {
    configProvider: ConfigurationProvider
}

type SetConfigWorkflow = string -> Reader<ConfigEnvironment, Result<unit, ErrorText>>

type GetConfigWorkflow = string -> Reader<ConfigEnvironment, Result<Option<string>, ErrorText>>
let getConfigWorkflow: GetConfigWorkflow =
    fun requestedKey -> reader {
        let! services = Reader.ask
       
        return result {
            let! key = ConfigKey.create requestedKey            
            let! configurationText = services.configProvider()
            
            let! value = configurationText
                         |> Configuration.create
                         |> Configuration.getValue key    
            return value // TODO: why cant use return! - dig into extension code
        }     
    }
        