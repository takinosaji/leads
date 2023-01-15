module Leads.Shell.Commands.Config.Get

open System
open System.CommandLine
open System.Threading.Tasks

open Leads.Core.Config.Workflows
open Leads.Core.Utilities.Dependencies
open Leads.Core.Config.Workflows

open Leads.DrivenAdapters.ConfigAdapters

open Leads.Shell
open Leads.Shell.Utilities

let private handler = fun requestedKey ->
    reader {        
        getConfigWorkflow requestedKey
    } |> Reader.run {
        configProvider = yamlFileConfigurationProvider
    }
    
let appendGetConfigSubCommand: SubCommandAppender =
    fun cmd ->    
        let getConfigSubCommand = Command("get", "The get command retrieves the specific configuration value by key")   

        let keyArg = createArgument<string> "key" "Config Key"
        
        getConfigSubCommand.AddArgument keyArg
                  
        getConfigSubCommand.SetHandler(handler, keyArg)
        
        cmd.AddCommand getConfigSubCommand
        
        cmd      