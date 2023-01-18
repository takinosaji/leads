module Leads.Shell.Commands.Config.Get

open System
open System.CommandLine

open Leads.Core.Config
open Leads.Core.Config.Workflows
open Leads.Core.Utilities.ConstrainedTypes
open Leads.Core.Utilities.Dependencies

open Leads.DrivenAdapters.ConfigAdapters

open Leads.Shell
open Leads.Shell.Utilities

let private printValue = function
    | Ok (Some (value:string)) ->
        Console.WriteLine(value)
    | Ok None ->
        Console.WriteLine("The config value is not set")
    | Error (ErrorText error) ->
        Console.WriteLine error
        
let private handler = fun requestedKey ->
    reader {        
        let! configValue = getConfigWorkflow requestedKey
        configValue |> printValue        
    } |> Reader.run {
        provideConfig = provideJsonFileConfiguration
        applyConfigValue = applyJsonFileConfiguration
    }
    
let appendGetConfigSubCommand: SubCommandAppender =
    fun cmd ->    
        let getConfigSubCommand = Command("get", "The get command retrieves the specific configuration value by key")   

        let keyArg = createArgument<string> "key" "Config Key"
        keyArg.AddCompletions(fun _ -> ConfigKey.AllowedConfigKeys |> Seq.ofList) |> ignore
        
        getConfigSubCommand.AddArgument keyArg
                  
        getConfigSubCommand.SetHandler(handler, keyArg)
        
        cmd.AddCommand getConfigSubCommand
        
        cmd      