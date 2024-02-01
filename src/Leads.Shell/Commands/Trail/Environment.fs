module Leads.Shell.Commands.Trail.Environment

open Leads.Core.Forests.Services
open Leads.Core.Forests.Workflows

open Leads.SecondaryAdapters.JsonFileBased.ForestAdapters

open Leads.Shell.ShellEnvironment
open Leads.Shell.Commands.Config.Environment

let private trailSecondaryAdapters = createLocalJsonFileTrailAdapters variables.defaultWorkingDirPath

let findForestEnvironment: FindForestEnvironment = {
    provideAllowedConfigKeys = getConfigEnvironment.provideAllowedConfigKeys
    provideConfig = getConfigEnvironment.provideConfig
    findForests = trailSecondaryAdapters.findForests
}