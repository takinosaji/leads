module Leads.Shell.Commands.Config.Set

open System
open System.CommandLine

open Leads.Core.Config.Workflows
open Leads.Utilities.Dependencies

open Leads.Shell
open Leads.Shell.Utilities

let private printSetResult = function
    | Ok _ -> ()
    | Error (errorText:string) ->
        Console.WriteLine errorText
        
let private handler = fun keyString newValueString ->
    reader {        
        let! setResult = setConfigValueWorkflow keyString newValueString
        setResult |> printSetResult        
    } |> Reader.run Environment.setConfigValueEnvironment

let appendConfigSetSubCommand: SubCommandAppender =
    fun cmd ->    
        let getConfigSubCommand = Command("set", "The set command updates the specific configuration key with the value")   

        let keyArg = createArgument<string> "key" "Config Key"
        let valueArg = createArgument<string> "value" "Config Value"
        
        getConfigSubCommand.AddArgument keyArg
        getConfigSubCommand.AddArgument valueArg
                  
        getConfigSubCommand.SetHandler(handler, keyArg, valueArg)
        
        cmd.AddCommand getConfigSubCommand
        
        cmd      