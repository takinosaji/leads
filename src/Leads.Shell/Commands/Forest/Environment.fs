module Leads.Shell.Commands.Forest.Environment

open Leads.Core.Forests.Services
open Leads.Core.Forests.Workflows

open Leads.DrivenAdapters.ForestAdapters

open Leads.Shell.Environment
open Leads.Shell.Commands.Config.Environment

let getForestsEnvironment: ListForestsEnvironment = {
        defaultWorkingDirPath = shellEnvironment.defaultWorkingDirPath
        provideForests = provideJsonFileForests
        provideConfig = getConfigEnvironment.provideConfig
    }

let addForestEnvironment: AddForestEnvironment = {
    defaultWorkingDirPath = shellEnvironment.defaultWorkingDirPath
    provideForests = provideJsonFileForests
    provideConfig = getConfigEnvironment.provideConfig
    addForest = addForestToJsonFile
} 