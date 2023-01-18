module Leads.Core.Config.Workflows

open Leads.Core.Utilities.ConstrainedTypes
open Leads.Core.Utilities.Result
open Leads.Core.Utilities.Dependencies
open Leads.Core.Config


type ConfigurationProvider = unit -> Result<ConfigInputDto, ErrorText>
type ConfigurationValueApplier = ConfigKey -> ConfigValue -> Result<unit, ErrorText>
type ConfigEnvironment = {
    provideConfig: ConfigurationProvider
    applyConfigValue: ConfigurationValueApplier
}

type ConfigValueOutputDto = Option<string>
type GetConfigWorkflow = string -> Reader<ConfigEnvironment, Result<ConfigValueOutputDto, ErrorText>>
let getConfigWorkflow: GetConfigWorkflow = 
    fun requestedKey -> reader {
        let! services = Reader.ask
       
        return result {
            let! key = ConfigKey.create requestedKey            
            let! unvalidatedConfiguration = services.provideConfig()
            
            let! value = unvalidatedConfiguration
                         |> Configuration.create
                         |> Configuration.getValue key    
            return value // TODO: why cant use return! - dig into extension code
        }     
    }

// TODO: Write unit tests
type SetConfigWorkflow = string -> string -> Reader<ConfigEnvironment, Result<unit, ErrorText>>
let setConfigWorkflow: SetConfigWorkflow =
    fun keyToUpdateString newValueString -> reader {
        let! services = Reader.ask
       
        return result {
            let! key = ConfigKey.create keyToUpdateString
            let! value = ConfigValue.create newValueString
            let! updateResult = services.applyConfigValue key value
            
            return updateResult
        }
    }
    
// TODO: Write unit tests
type ListConfigWorkflow = unit -> Reader<ConfigEnvironment, ConfigOutputDto>
let listConfigWorkflow: ListConfigWorkflow = 
    fun _ -> reader {
        let! services = Reader.ask
       
        return result {     
            let! unvalidatedConfiguration = services.provideConfig()
            let configuration = unvalidatedConfiguration |> Configuration.create
            let configOutput = configuration
        }     
    }