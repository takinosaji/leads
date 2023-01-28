module Leads.Shell.Commands.Config.Environment

open Leads.Shell.Environment
open Leads.Core.Config.Workflows
open Leads.DrivenAdapters.ConfigAdapters

let environmentGet: GetConfigEnvironment = {
        configFilePath = shellEnvironment.configFilePath
        provideConfig = provideJsonFileConfiguration
    }

let environmentSet: SetConfigEnvironment = {
        configFilePath = shellEnvironment.configFilePath
        applyConfigValue = applyJsonFileConfiguration
    }