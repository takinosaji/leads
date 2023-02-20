module Leads.Shell.Commands.Config.Environment

open Leads.Core.Config.Services
open Leads.Core.Config.Workflows

open Leads.DrivenAdapters.FileBased.ConfigAdapters

open Leads.Shell.Environment

let private configFilePath = $"{shellEnvironment.defaultWorkingDirPath}/config.json";
let private configDrivenAdapters = createLocalJsonFileConfigAdapters configFilePath

let getConfigEnvironment: GetConfigEnvironment = {
        provideConfig = configDrivenAdapters.provideConfiguration
    }

let setConfigValueEnvironment: SetConfigEnvironment = {
        applyConfigValue = configDrivenAdapters.applyConfigValue
    }