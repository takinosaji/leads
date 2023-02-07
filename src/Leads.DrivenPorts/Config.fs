namespace Leads.DrivenPorts.Config

module DTO = 
    type ConfigDrivenDto = Map<string, string> Option

    type ConfigEntryDto =
        | ValidEntryDto of {| Key: string; Value: string |}
        | InvalidKeyDto of {| Key: string; Error: string |}
        | InvalidValueDto of {| Key: string; Value: string; Error: string |}
    type ConfigDrivingDto = ConfigEntryDto list option

open DTO    

type ConfigurationProvider = unit -> Result<ConfigDrivenDto, string>
type ConfigurationValueApplier = string -> string -> Result<unit, string>