module Leads.Shell.Commands.Forest.Environment

open Leads.Core.Forests.Services
open Leads.Core.Forests.Workflows

open Leads.SecondaryAdapters.FileBased.ForestAdapters

open Leads.Shell.Environment
open Leads.Shell.Commands.Config.Environment

let private forestSecondaryAdapters = createLocalJsonFileForestAdapters shellEnvironment.defaultWorkingDirPath

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

