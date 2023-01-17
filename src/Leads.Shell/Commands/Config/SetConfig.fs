module Leads.Shell.Commands.Config.Set

open System
open System.CommandLine

open Leads.Core.Config.Workflows
open Leads.Core.Utilities.ConstrainedTypes
open Leads.Core.Utilities.Dependencies

open Leads.DrivenAdapters.ConfigAdapters

open Leads.Shell
open Leads.Shell.Utilities

let private printSetResult = function
    | Ok _ -> ()
    | Error (ErrorText error) ->
        Console.WriteLine error
        
let private handler = fun keyString newValueString ->
    reader {        
        let! setResult = setConfigWorkflow keyString newValueString
        setResult |> printSetResult        
    } |> Reader.run {
        configProvider = yamlFileConfigurationProvider
        configApplier = yamlFileConfigurationApplier
    }

let appendSetConfigSubCommand: SubCommandAppender =
    fun cmd ->    
        let getConfigSubCommand = Command("set", "The set command updates the specific configuration key with the value")   

        let keyArg = createArgument<string> "key" "Config Key"
        let valueArg = createArgument<string> "value" "Config Value"
        
        getConfigSubCommand.AddArgument keyArg
        getConfigSubCommand.AddArgument valueArg
                  
        getConfigSubCommand.SetHandler(handler, keyArg, valueArg)
        
        cmd.AddCommand getConfigSubCommand
        
        cmd      