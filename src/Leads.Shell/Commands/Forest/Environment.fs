﻿module Leads.Shell.Commands.Forest.Environment

open Leads.Shell.Environment
open Leads.Core.Forests.Workflows

open Leads.DrivenAdapters.ConfigAdapters
open Leads.DrivenAdapters.ForestAdapters

let environment = {
        defaultWorkingDirPath = shellEnvironment.defaultWorkingDirPath
        configFilePath = shellEnvironment.configFilePath
        provideForests = provideJsonFileForests
        provideConfig = provideJsonFileConfiguration
    }