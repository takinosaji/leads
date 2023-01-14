module Leads.Core.Config.Models

open Leads.Core.Models

type ConfigKey = ConfigKey of string
type ConfigValue = ConfigValue of string

type Configuration = {
    DefaultStream: string
    WorkingDir: string
}