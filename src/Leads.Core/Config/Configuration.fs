namespace Leads.Core.Config

open Leads.Core.Utilities.ConstrainedTypes
open Leads.Core.Config

type ConfigurationSource = Map<string, string> Option

type ValidEntry = {
    Key: ConfigKey
    Value: ConfigValue
}

type InvalidKey = {
    KeyString: string
    Error: ErrorText
}

type InvalidValue = {
    Key: ConfigKey
    ValueString: string
    Error: ErrorText
}

type ConfigEntry =
    | ValidEntry of ValidEntry
    | InvalidKey of InvalidKey
    | InvalidValueEntry of InvalidValue
type Configuration = private Configuration of ConfigEntry list Option
module Configuration =           
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