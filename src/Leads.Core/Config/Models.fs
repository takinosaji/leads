module Leads.Core.Config.Models

open Leads.Core.Utilities.ConstrainedTypes

type ConfigurationSource = Map<string, string> Option

module ConfigKey =
    type ConfigKey = private ConfigKey of key:string
    
    let private allowedConfigKeys = [
        "default.stream";
        "working.dir"
    ]
    
    let create (keyString:string) =
        createPredefinedString (nameof(ConfigKey)) ConfigKey (keyString.ToLower()) allowedConfigKeys           
    let value (ConfigKey key) = key    

module ConfigValue =
    type ConfigValue = private ConfigValue of value:string
    
    let create (valueString:string) =
        createLimitedString (nameof(ConfigValue)) ConfigValue 50 valueString           
    let value (ConfigValue value) = value   


module Configuration =
    type ValidEntry = {
        Key: ConfigKey.ConfigKey
        Value: ConfigValue.ConfigValue
    }

    type InvalidKey = {
        KeyString: string
        Error: ErrorText
    }

    type InvalidValue = {
        Key: ConfigKey.ConfigKey
        ValueString: string
        Error: ErrorText
    }

    type ConfigEntry =
        | ValidEntry of ValidEntry
        | InvalidKey of InvalidKey
        | InvalidValueEntry of InvalidValue
        
    type Configuration = private Configuration of ConfigEntry list Option
    
    type ConfigurationFactory = ConfigurationSource -> Configuration
    let create: ConfigurationFactory = function
        | Some stringMap ->
            stringMap
            |> Map.toList
            |> List.map (fun textEntry ->
                let configKey = ConfigKey.create(fst textEntry)
                let configValue = ConfigValue.create(snd textEntry)
                
                match configKey, configValue with
                | Ok key, Ok value ->
                    ValidEntry { Key = key; Value = value }
                | Error keyError, _ ->
                    InvalidKey { KeyString = fst textEntry; Error = keyError }
                | Ok key, Error valueError ->
                    InvalidValueEntry { Key = key; ValueString = snd textEntry; Error = valueError })
            |> Some
            |> Configuration
        | None -> None
                  |> Configuration
    
    let value (Configuration configuration) = configuration
    
    let getValue key (configuration: Configuration) =
        match (value configuration) with
        | Some configEntries ->
            let entry = List.tryFind (fun i ->
                match i with
                | ValidEntry entry -> entry.Key = key
                | InvalidValueEntry entry -> entry.Key = key
                | _ -> false) configEntries
            
            match entry with
                | Some(ValidEntry validEntry) -> Ok(Some (validEntry.Value |> ConfigValue.value))
                | Some(InvalidValueEntry invalidEntry) -> Error invalidEntry.Error
                | Some _
                | None -> Ok None
        | None -> Ok None
