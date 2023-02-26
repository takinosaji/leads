module Leads.Core.Config.Workflows

open Leads.Core.Config.ConfigDTO
open Leads.SecondaryPorts.Config
open Leads.Utilities.ConstrainedTypes
open Leads.Utilities.Result
open Leads.Utilities.Dependencies
open Leads.Core.Config.ConfigValueDTO
open Leads.Core.Config.Services

type SetConfigEnvironment = {
    provideAllowedKeys: AllowedConfigKeysProvider
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
    fun keyToUpdate newValue -> reader {
        let! environment = Reader.ask
        
        return
            match List.contains <| keyToUpdate <| environment.provideAllowedKeys() with
            | true ->
                result {
                    let! key = ConfigKey.create keyToUpdate
                    let! value = ConfigValue.create newValue
                    let! updateResult =
                        environment.applyConfigValue (ConfigKey.value key) (ConfigValue.value value)
                        |> Result.mapError stringToErrorText
                    
                    return updateResult
                } |> Result.mapError errorTextToString
            | false ->
                Error $"Configuration key {keyToUpdate} is not allowed"  
        } 
        
// TODO: Write unit tests
type ListConfigWorkflow = unit -> Reader<GetConfigEnvironment, Result<ConfigPrimaryDto, string>>
let listConfigWorkflow: ListConfigWorkflow = 
    fun () -> reader {
        let! config = getConfig()
        return config
            |> Result.map Configuration.toPrimaryDto
            |> Result.mapError errorTextToString   
    }   