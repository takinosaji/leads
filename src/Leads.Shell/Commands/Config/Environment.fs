module Leads.Shell.Commands.Config.Environment

open Leads.Core.Config.Services
open Leads.Core.Config.Workflows

open Leads.SecondaryAdapters.FileBased.ConfigAdapters

open Leads.Shell.Environment

let private configFilePath = $"{shellEnvironment.defaultWorkingDirPath}/config.json";
let private configSecondaryAdapters = createLocalJsonFileConfigAdapters configFilePath

let getConfigEnvironment: GetConfigEnvironment = {
        provideConfig = configSecondaryAdapters.provideConfiguration
    }

let setConfigValueEnvironment: SetConfigEnvironment = {
        applyConfigValue = configSecondaryAdapters.applyConfigValue
    }