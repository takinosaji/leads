module Leads.Shell.Commands.Config.Environment

open Leads.Core.Config.Services
open Leads.Core.Config.Workflows

open Leads.SecondaryAdapters.JsonFileBased.ConfigAdapters

open Leads.Shell.ShellEnvironment

let private configFilePath = $"{variables.defaultWorkingDirPath}/config.json";
let private configSecondaryAdapters = createLocalJsonFileConfigAdapters configFilePath

let getConfigEnvironment: GetConfigEnvironment = {
        provideAllowedConfigKeys = configSecondaryAdapters.provideAllowedKeys
        provideConfig = configSecondaryAdapters.provideConfiguration
    }

let setConfigValueEnvironment: SetConfigEnvironment = {
        provideAllowedConfigKeys = configSecondaryAdapters.provideAllowedKeys
        applyConfigValue = configSecondaryAdapters.applyConfigValue
    }