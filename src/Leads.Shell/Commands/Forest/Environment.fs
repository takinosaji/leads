module Leads.Shell.Commands.Forest.Environment

open Leads.Core.Forests.Workflows

open Leads.DrivenAdapters.ForestAdapters

open Leads.Shell.Environment
open Leads.Shell.Commands.Config.Environment

let environment: ForestEnvironment = {
        defaultWorkingDirPath = shellEnvironment.defaultWorkingDirPath
        provideForests = provideJsonFileForests
        provideConfig = getConfigEnvironment.provideConfig
    }