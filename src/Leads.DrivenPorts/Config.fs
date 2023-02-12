namespace Leads.DrivenPorts.Config

module DTO = 
    type ConfigDrivenDto = Map<string, string> Option
open DTO    

type ConfigurationProvider = unit -> Result<ConfigDrivenDto, string>
type ConfigurationValueApplier = string -> string -> Result<unit, string>