module Leads.Core.Config.Models

open Leads.Core.Utilities.ConstrainedTypes

type StringMap = Map<string, string>

module ConfigKey =
    type ConfigKey = private ConfigKey of key:string
    
    let private allowedConfigKeys = [
        "DefaultStream";
        "WorkingDir"
    ]
    
    let create keyString =
        createPredefinedString ConfigKey keyString allowedConfigKeys           
    let value (ConfigKey key) = key    


type ConfigValue = ConfigValue of value:string
let configValueFactory value: Result<ConfigValue, ErrorText> = Ok(ConfigValue value) // TODO: replace with smart constructor

module Configuration =
    type ValidEntry = {
        Key: ConfigKey.ConfigKey
        Value: ConfigValue
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
        
    type Configuration = ConfigEntry list // TODO: hide constructor

    let getValue key configuration =
        let entry = List.tryFind (fun i ->
            match i with
            | ValidEntry entry -> entry.Key = key
            | InvalidValueEntry entry -> entry.Key = key
            | _ -> false) configuration
        
        match entry with
            | Some(ValidEntry validEntry) -> Ok(Some validEntry.Value)
            | Some(InvalidValueEntry invalidEntry) -> Error invalidEntry.Error
            | Some _
            | None -> Ok None

    type ConfigurationFactory = StringMap -> Configuration
    let create: ConfigurationFactory =
        fun (stringMap:StringMap) ->
            stringMap
            |> Map.toList
            |> List.map (fun textEntry ->
                let configKey = ConfigKey.create(fst textEntry)
                let configValue = configValueFactory(snd textEntry)
                
                match configKey, configValue with
                | Ok key, Ok value ->
                    ValidEntry { Key = key; Value = value }
                | Error keyError, _ ->
                    InvalidKey { KeyString = fst textEntry; Error = keyError }
                | Ok key, Error valueError ->
                    InvalidValueEntry { Key = key; ValueString = snd textEntry; Error = valueError }
        )