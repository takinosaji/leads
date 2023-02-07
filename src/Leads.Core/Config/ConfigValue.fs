namespace Leads.Core.Config

open Leads.Utilities.ConstrainedTypes

module DTO = 
    type OptionalConfigValueDto = Option<string>

type ConfigValue = private ConfigValue of value:string
type OptionalConfigValue = ConfigValue option

module ConfigValue =
    let create (valueString:string) =
        createLimitedString (nameof(ConfigValue)) ConfigValue 50 valueString           
    let value (ConfigValue value) = value
    
    let optionValue optionValue =
        Option.map (fun v -> value v) optionValue
        
    let valueOrDefaultOption valueOption defaultValue =
        match valueOption with
        | Some value -> value
        | None -> ConfigValue defaultValue
        
        
    


