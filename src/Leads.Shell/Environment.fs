module Leads.Shell.Environment

open System

let shellEnvironment = {|
        defaultWorkingDirPath = $"{Environment.GetFolderPath Environment.SpecialFolder.UserProfile}/.leads"
    |}
