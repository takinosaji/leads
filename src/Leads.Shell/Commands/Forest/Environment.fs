module Leads.Shell.Commands.Forest.Environment

open Leads.Core.Forests.Services
open Leads.Core.Forests.Workflows

open Leads.DrivenAdapters.ForestAdapters

open Leads.Shell.Environment
open Leads.Shell.Commands.Config.Environment

let private forestDrivenAdapters = createLocalJsonFileForestAdapters shellEnvironment.defaultWorkingDirPath

let getForestsEnvironment: ListForestsEnvironment = {
        provideConfig = getConfigEnvironment.provideConfig
        provideForests = forestDrivenAdapters.provideJsonFileForests
    }

let addForestEnvironment: AddForestEnvironment = {
    provideForests = getForestsEnvironment.provideForests
    provideConfig = getForestsEnvironment.provideConfig
    addForest = forestDrivenAdapters.addForest
}