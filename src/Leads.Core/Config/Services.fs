module Leads.Core.Config.Services

open Leads.Core.Utilities.ConstrainedTypes
open Leads.Core.Utilities.Result
open Leads.Core.Utilities.Dependencies

type ConfigurationProvider = string -> Result<ConfigInputDto, ErrorText>

type GetConfigEnvironment = {
    configFilePath: string
    provideConfig: ConfigurationProvider
}

type internal GetConfigValue = string -> Reader<GetConfigEnvironment, Result<OptionalConfigValue, ErrorText>>
let internal getConfigValue: GetConfigValue =
    fun requestedKey -> reader {
        let! environment = Reader.ask
       
        return result {
            let! key = ConfigKey.create requestedKey            
            let! unvalidatedConfiguration = environment.provideConfig environment.configFilePath
            
            let! value = unvalidatedConfiguration
                         |> Configuration.create
                         |> Configuration.keyValue key
            return value // TODO: why cant use return! - dig into extension code
        }  
    }