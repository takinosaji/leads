namespace Leads.Core.Config

open Leads.Core.Utilities.ConstrainedTypes
open Leads.Core.Config

type ConfigInputDto = Map<string, string> Option

type ConfigEntryDto =
    | ValidEntryDto of {| Key: string; Value: string |}
    | InvalidKeyDto of {| Key: string; Error: string |}
    | InvalidValueDto of {| Key: string; Value: string; Error: string |}
type ConfigOutputDto = ConfigEntryDto list option

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
    | InvalidValue of InvalidValue
    
type Configuration = private Configuration of ConfigEntry list option

module Configuration =           
    type ConfigurationFactory = ConfigInputDto -> Configuration
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
                    InvalidValue { Key = key; ValueString = snd textEntry; Error = valueError })
            |> Some
            |> Configuration
        | None -> None
                  |> Configuration
    
    let value (Configuration configuration) = configuration
    
    let keyValue key (configuration: Configuration) =
        match (value configuration) with
        | Some configEntries ->
            let entry = List.tryFind (fun i ->
                match i with
                | ValidEntry entry -> entry.Key = key
                | InvalidValue entry -> entry.Key = key
                | _ -> false) configEntries
            
            match entry with
                | Some(ValidEntry validEntry) -> Ok(Some (validEntry.Value))
                | Some(InvalidValue invalidEntry) -> Error invalidEntry.Error
                | Some _
                | None -> Ok None
        | None -> Ok None
        
    let toOutputDto (configuration: Configuration) :ConfigOutputDto =        
        match value configuration with
        | Some configEntries ->
            configEntries
            |> List.map (function
                | ValidEntry ve ->
                    let key = ConfigKey.value ve.Key
                    let value = ConfigValue.value ve.Value
                    ValidEntryDto {| Key = key; Value = value |}
                | InvalidKey ik ->
                    let (ErrorText errorText) = ik.Error
                    InvalidKeyDto {| Key = ik.KeyString; Error = errorText |}
                | InvalidValue iv ->
                    let key = ConfigKey.value iv.Key
                    let (ErrorText errorText) = iv.Error
                    InvalidValueDto {| Key = key; Value = iv.ValueString; Error = errorText |}
                )
            |> Some
        | None -> None