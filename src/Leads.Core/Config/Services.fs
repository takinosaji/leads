module Leads.Core.Config.Services

open Leads.SecondaryPorts.Config
open Leads.Utilities.ConstrainedTypes
open Leads.Utilities.Result
open Leads.Utilities.Dependencies

type GetConfigEnvironment = {
    provideConfig: ConfigurationProvider
}

type internal GetConfigValue = string -> Reader<GetConfigEnvironment, Result<OptionalConfigValue, ErrorText>>
let internal getConfigValue: GetConfigValue =
    fun requestedKey -> reader {
        let! environment = Reader.ask
       
        return result {
            let! key = ConfigKey.create requestedKey            
            let! unvalidatedConfiguration =
                environment.provideConfig() |> Result.mapError stringToErrorText
            
            let! value = unvalidatedConfiguration
                         |> Configuration.create
                         |> Configuration.keyValue key
            return value // TODO: why cant use return! - dig into extension code
        }  
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