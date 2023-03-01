module Leads.Shell.Commands.Forest.Use

open System
open System.CommandLine

open Leads.Core.Forests
open Leads.Core.Forests.Forest.DTO
open Leads.Shell.Commands.Config.Environment
open Leads.Shell.Commands.Forest.Environment
open Leads.Shell.Commands.Forest.Utilities

open Leads.Core.Config.Workflows
open Leads.Core.Forests.Workflows
open Leads.Utilities.Dependencies

open Leads.Shell
open Leads.Shell.Utilities

let private printSetResult (forest:ForestPODto) = function
    | Ok _ ->
        writeLine $"Forest {forest.Name} [{forest.Hash}] has been set as default"
    | Error (errorText:string) ->
        Console.WriteLine errorText

let private printMultipleForestsError searchText forests =
    printForestsWithColoredText "Found Forests" [(searchText, "red")] forests
    writeErrorLine $"Multiple active forests found by the search text provided. You can have only one default forests set up"

let private handler = fun searchText ->
    let statuses = ForestStatuses.composeStatuses false false false false
    let findForestsResult = describeForestsWorkflow searchText statuses |> Reader.run findForestEnvironment
    
    // TODO: Move to application core
    match findForestsResult with
    | Ok forestsOption ->
        match forestsOption with
        | None ->
            writeErrorLine $"Any active forests with Name of Hash containing {searchText} has not been found"
        | Some [forest] ->
            let setResult = setConfigValueWorkflow "default.forest" forest.Hash |> Reader.run setConfigValueEnvironment
            setResult |> printSetResult forest   
        | Some forests ->
            forests |> printMultipleForestsError searchText
    | Error errorText ->
        errorText |> writeErrorLine

let appendForestUseSubCommand: SubCommandAppender =
    fun cmd ->    
        let useForestSubCommand = Command("use", "The use sets default forest if it exists")   
        let searchText = createArgument<string> "searchText" "Provide the complete or partial forest hash or name"
        
        useForestSubCommand.AddArgument searchText             
        useForestSubCommand.SetHandler(handler, searchText)
        
        cmd.AddCommand useForestSubCommand
        cmd      