module Leads.Shell.Commands.Forest.Environment

open Leads.Core.Forests.Services
open Leads.Core.Forests.Workflows

open Leads.SecondaryAdapters.FileBased.ForestAdapters

open Leads.Shell.Environment
open Leads.Shell.Commands.Config.Environment

let private forestSecondaryAdapters = createLocalJsonFileForestAdapters shellEnvironment.defaultWorkingDirPath

let findForestEnvironment: FindForestEnvironment = {
    provideConfig = getConfigEnvironment.provideConfig
    findForests = forestSecondaryAdapters.findForests
}

let addForestEnvironment: AddForestEnvironment = {
    provideConfig = findForestEnvironment.provideConfig
    addForest = forestSecondaryAdapters.addForest
}

// let completeForestEnvironment: CompleteForestEnvironment = {
//     provideConfig = getForestsEnvironment.provideConfig
//     completeForest = forestSecondaryAdapters.completeForest
// }

