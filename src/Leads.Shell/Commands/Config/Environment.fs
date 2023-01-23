module Leads.Shell.Commands.Config.Environment

open Leads.Shell.Environment
open Leads.Core.Config.Workflows
open Leads.DrivenAdapters.ConfigAdapters

let environment = {
        configFilePath = environment.configFilePath
        provideConfig = provideJsonFileConfiguration
        applyConfigValue = applyJsonFileConfiguration
    }