module Leads.Core.Config.Models

open Leads.Core.Models
open Leads.Core.Utilities.Result

type ConfigKey = ConfigKey of key:string
let configKeyFactory key: Result<ConfigKey, ErrorText> = Ok(ConfigKey key)

type ConfigValue = ConfigValue of value:string
let configValueFactory value: Result<ConfigValue, ErrorText> = Ok(ConfigValue value)

type StringMap = Map<string, string>

module ConfigKeys = 
    let DefaultStream = "DefaultStream"
    let WorkingDir = "WorkingDir"
    
type Configuration = Configuration of Map<string, string>
let configFactory (stringMap:StringMap) =
    result {
        for stringKey in stringMap.Keys do
            let! configKey = configKeyFactory stringKey
            let! configValue = configValueFactory stringMap[stringKey]
            
        return (Configuration stringMap)
    }
        