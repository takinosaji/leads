namespace Leads.Core.Config

open Leads.Core.Utilities.ConstrainedTypes

type ConfigKey = private ConfigKey of key:string
module ConfigKey =
    let DefaultForestKey = "default.forest"
    let WorkingDirKey = "working.dir"
    let AllowedConfigKeys = [ DefaultForestKey; WorkingDirKey ]
    
    let create (keyString:string) =
        createPredefinedString (nameof(ConfigKey)) ConfigKey (keyString.ToLower()) AllowedConfigKeys           
    let value (ConfigKey key) = key


