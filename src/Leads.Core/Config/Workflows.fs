module Leads.Core.Config.Workflows

open Leads.Core.Utilities.ConstrainedTypes
open Leads.Core.Utilities.Result
open Leads.Core.Utilities.Dependencies
open Leads.Core.Config

type ConfigurationProvider = unit -> Result<ConfigurationSource, ErrorText>
type ConfigurationApplier = ConfigKey -> ConfigValue -> Result<unit, ErrorText>
type ConfigEnvironment = {
    configProvider: ConfigurationProvider
    configApplier: ConfigurationApplier
}

type ConfigValueOutput = Result<Option<string>, ErrorText>
type GetConfigWorkflow = string -> Reader<ConfigEnvironment, ConfigValueOutput>
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

type SetConfigWorkflow = string -> string -> Reader<ConfigEnvironment, Result<unit, ErrorText>>
let setConfigWorkflow: SetConfigWorkflow =
    fun keyToUpdateString newValueString -> reader {
        let! services = Reader.ask
       
        return result {
            let! key = ConfigKey.create keyToUpdateString
            let! value = ConfigValue.create newValueString
            let! updateResult = services.configApplier key value
            
            return updateResult
        }
    }     