namespace Leads.Core.Config

open Leads.Core.Utilities.ConstrainedTypes

type ConfigKey = private ConfigKey of key:string
module ConfigKey =    
    let AllowedConfigKeys = [
        "default.stream";
        "working.dir"
    ]
    
    let create (keyString:string) =
        createPredefinedString (nameof(ConfigKey)) ConfigKey (keyString.ToLower()) AllowedConfigKeys           
    let value (ConfigKey key) = key    


