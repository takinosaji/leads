module Leads.Shell.Environment

open System

let environment = {|
        defaultWorkingDirPath = $"{Environment.GetFolderPath Environment.SpecialFolder.UserProfile}/leads"
        configFilePath = $"{Environment.GetFolderPath Environment.SpecialFolder.UserProfile}/leads/config.yaml"
    |}
