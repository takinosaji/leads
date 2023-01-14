module Leads.Core.Config.Workflows

open Leads.Core.Models
open Leads.Core.Config.Models
open Leads.Core.Utilities.Result

type GetConfigValue = ConfigKey -> TaskResult<ConfigValue, Error>
type SetConfigValue = ConfigKey -> TaskResult<unit, Error>


type ConfigurationFactory = unit -> TaskResult<Option<Configuration>, string>  

