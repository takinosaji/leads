module Leads.Core.Config.Services

open Leads.SecondaryPorts.Config
open Leads.Utilities.ConstrainedTypes
open Leads.Utilities.Result
open Leads.Utilities.Dependencies

type GetConfigEnvironment = {
    provideAllowedConfigKeys: AllowedConfigKeysProvider
    provideConfig: ConfigurationProvider
}

type internal GetConfigValue = string -> Reader<GetConfigEnvironment, Result<OptionalConfigValue, ErrorText>>
let internal getConfigValue: GetConfigValue =
    fun requestedKey -> reader {
        let! environment = Reader.ask
       
        return
            match List.contains <| requestedKey <| environment.provideAllowedConfigKeys() with
            | true ->
                result {
                    let! key = ConfigKey.create requestedKey            
                    let! unvalidatedConfiguration =
                        environment.provideConfig() |> Result.mapError stringToErrorText
                    
                    let! value = unvalidatedConfiguration
                                 |> Configuration.create
                                 |> Configuration.keyValue key
                    return value
                }
            | false ->
                Error (ErrorText $"Configuration key {requestedKey} is not allowed")        
        }
    
type internal GetConfig = unit -> Reader<GetConfigEnvironment, Result<Configuration, ErrorText>>
let internal getConfig: GetConfig = 
    fun _ -> reader {
        let! services = Reader.ask
       
        return result {     
            let! unvalidatedConfiguration = services.provideConfig() |> Result.mapError stringToErrorText
            return unvalidatedConfiguration
                |> Configuration.create       
        }    
    }