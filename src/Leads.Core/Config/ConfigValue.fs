namespace Leads.Core.Config

open Leads.Core.Utilities.ConstrainedTypes

type ConfigValueOutputDto = Option<string>

type ConfigValue = private ConfigValue of value:string

module ConfigValue =
    let create (valueString:string) =
        createLimitedString (nameof(ConfigValue)) ConfigValue 50 valueString           
    let value (ConfigValue value) = value
    
    let optionValue optionValue =
        Option.map (fun v -> value v) optionValue


