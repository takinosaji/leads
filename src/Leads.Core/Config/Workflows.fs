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

type GetConfigWorkflow = string -> Reader<ConfigEnvironment, Result<ConfigValueOutputDto, string>>
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
        } |> Result.mapError errorTextToString     
    }

// TODO: Write unit tests
type SetConfigWorkflow = string -> string -> Reader<ConfigEnvironment, Result<unit, string>>
let setConfigWorkflow: SetConfigWorkflow =
    fun keyToUpdateString newValueString -> reader {
        let! services = Reader.ask
       
        return result {
            let! key = ConfigKey.create keyToUpdateString
            let! value = ConfigValue.create newValueString
            let! updateResult = services.applyConfigValue key value
            
            return updateResult
        } |> Result.mapError errorTextToString
    }
    
// TODO: Write unit tests
type ListConfigWorkflow = unit -> Reader<ConfigEnvironment, Result<ConfigOutputDto, string>>
let listConfigWorkflow: ListConfigWorkflow = 
    fun () -> reader {
        let! services = Reader.ask
       
        return result {     
            let! unvalidatedConfiguration = services.provideConfig()
            return unvalidatedConfiguration
                |> Configuration.create
                |> Configuration.toOutputDto            
        } |> Result.mapError errorTextToString     
    }