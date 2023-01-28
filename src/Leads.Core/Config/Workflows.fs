module Leads.Core.Config.Workflows

open Leads.Core.Utilities.ConstrainedTypes
open Leads.Core.Utilities.Result
open Leads.Core.Utilities.Dependencies
open Leads.Core.Config.DTO
open Leads.Core.Config.Services

type ConfigurationValueApplier = ConfigKey -> ConfigValue -> string -> Result<unit, ErrorText>

type SetConfigEnvironment = {
    configFilePath: string
    applyConfigValue: ConfigurationValueApplier
}

type GetConfigValueWorkflow = string -> Reader<GetConfigEnvironment, Result<OptionalConfigValueDto, string>>
let getConfigValueWorkflow: GetConfigValueWorkflow = 
    fun requestedKey -> reader {
        let! value = getConfigValue(requestedKey)
        return value
               |> Result.map ConfigValue.optionValue
               |> Result.mapError errorTextToString     
    }

// TODO: Write unit tests
type SetConfigValueWorkflow = string -> string -> Reader<SetConfigEnvironment, Result<unit, string>>
let setConfigValueWorkflow: SetConfigValueWorkflow =
    fun keyToUpdateString newValueString -> reader {
        let! services = Reader.ask
       
        return result {
            let! key = ConfigKey.create keyToUpdateString
            let! value = ConfigValue.create newValueString
            let! updateResult = services.applyConfigValue key value services.configFilePath
            
            return updateResult
        } |> Result.mapError errorTextToString
    }
        
// TODO: Write unit tests
type ListConfigWorkflow = unit -> Reader<GetConfigEnvironment, Result<ConfigOutputDto, string>>
let listConfigWorkflow: ListConfigWorkflow = 
    fun () -> reader {
        let! services = Reader.ask
       
        return result {     
            let! unvalidatedConfiguration = services.provideConfig services.configFilePath
            return unvalidatedConfiguration
                |> Configuration.create
                |> Configuration.toOutputDto            
        } |> Result.mapError errorTextToString     
    }