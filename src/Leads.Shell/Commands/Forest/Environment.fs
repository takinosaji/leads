module Leads.Shell.Commands.Forest.Environment

open Leads.Core.Forests.Services
open Leads.Core.Forests.Workflows

open Leads.SecondaryAdapters.JsonFileBased.ForestAdapters

open Leads.Shell.ShellEnvironment
open Leads.Shell.Commands.Config.Environment

let private forestSecondaryAdapters = createLocalJsonFileForestAdapters variables.defaultWorkingDirPath

let findForestEnvironment: FindForestEnvironment = {
    provideAllowedConfigKeys = getConfigEnvironment.provideAllowedConfigKeys
    provideConfig = getConfigEnvironment.provideConfig
    findForests = forestSecondaryAdapters.findForests
}

let addForestEnvironment: AddForestEnvironment = {
    provideAllowedConfigKeys = getConfigEnvironment.provideAllowedConfigKeys
    provideConfig = findForestEnvironment.provideConfig
    findForests = findForestEnvironment.findForests
    addForest = forestSecondaryAdapters.addForest
}

let updateForestEnvironment: UpdateForestEnvironment = {
    provideAllowedConfigKeys = getConfigEnvironment.provideAllowedConfigKeys
    provideConfig = findForestEnvironment.provideConfig
    findForests = forestSecondaryAdapters.findForests
    updateForest = forestSecondaryAdapters.updateForest
}

let deleteForestEnvironment: DeleteForestEnvironment = {
    provideAllowedConfigKeys = getConfigEnvironment.provideAllowedConfigKeys
    provideConfig = findForestEnvironment.provideConfig
    findForests = forestSecondaryAdapters.findForests
    deleteForest = forestSecondaryAdapters.deleteForest
}