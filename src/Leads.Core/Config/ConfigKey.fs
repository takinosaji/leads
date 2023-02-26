namespace Leads.Core.Config

open Leads.Utilities.ConstrainedTypes

type ConfigKey = private ConfigKey of key:string
module ConfigKey =   
    let create (keyString:string) =
        createLimitedString (nameof(ConfigKey)) ConfigKey 15 (keyString.ToLower())            
    let value (ConfigKey key) = key


