module Leads.Core.Config.Workflows

open Leads.DrivenPorts.Config
open Leads.DrivenPorts.Config.DTO
open Leads.Utilities.ConstrainedTypes
open Leads.Utilities.Result
open Leads.Utilities.Dependencies
open Leads.Core.Config.DTO
open Leads.Core.Config.Services

type SetConfigEnvironment = {
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
            let! updateResult =
                services.applyConfigValue (ConfigKey.value key) (ConfigValue.value value)
                |> Result.mapError stringToErrorText
            
            return updateResult
        } |> Result.mapError errorTextToString
    }
        
// TODO: Write unit tests
type ListConfigWorkflow = unit -> Reader<GetConfigEnvironment, Result<ConfigDrivingDto, string>>
let listConfigWorkflow: ListConfigWorkflow = 
    fun () -> reader {
        let! services = Reader.ask
       
        return result {     
            let! unvalidatedConfiguration = services.provideConfig()
            return unvalidatedConfiguration
                |> Configuration.create
                |> Configuration.toDrivingDto            
        }    
    }