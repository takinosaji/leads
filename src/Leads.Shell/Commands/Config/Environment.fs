module Leads.Shell.Commands.Config.Environment

open Leads.Core.Config.Services
open Leads.Core.Config.Workflows

open Leads.DrivenAdapters.ConfigAdapters

open Leads.Shell.Environment

let private configFilePath = $"{shellEnvironment.defaultWorkingDirPath}/config.json";
let private configDrivenAdapters = createLocalJsonConfigFileAdapters configFilePath

let getConfigEnvironment: GetConfigEnvironment = {
        provideConfig = configDrivenAdapters.provideJsonFileConfiguration
    }

let setConfigValueEnvironment: SetConfigEnvironment = {
        applyConfigValue = configDrivenAdapters.applyJsonFileConfiguration
    }