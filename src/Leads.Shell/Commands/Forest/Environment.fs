module Leads.Shell.Commands.Forest.Environment

open Leads.Core.Forests.Services
open Leads.Core.Forests.Workflows

open Leads.DrivenAdapters.FileBased.ForestAdapters

open Leads.Shell.Environment
open Leads.Shell.Commands.Config.Environment

let private forestDrivenAdapters = createLocalJsonFileForestAdapters shellEnvironment.defaultWorkingDirPath

let getForestsEnvironment: ListForestsEnvironment = {
        provideConfig = getConfigEnvironment.provideConfig
        provideForests = forestDrivenAdapters.provideForests
    }

let addForestEnvironment: AddForestEnvironment = {
    provideConfig = getForestsEnvironment.provideConfig
    addForest = forestDrivenAdapters.addForest
}

// let completeForestEnvironment: CompleteForestEnvironment = {
//     provideConfig = getForestsEnvironment.provideConfig
//     completeForest = forestDrivenAdapters.completeForest
// }

let describeForestEnvironment: DescribeForestEnvironment = {
    provideConfig = getForestsEnvironment.provideConfig
    findForest = forestDrivenAdapters.findForest
}