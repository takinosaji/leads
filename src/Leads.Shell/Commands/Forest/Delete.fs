module Leads.Shell.Commands.Forest.Delete

open System

open System.CommandLine

open Leads.Core.Forests.Workflows
open Leads.Utilities.Dependencies

open Leads.Shell
open Leads.Shell.Utilities
open Leads.Shell.Commands.Forest.Environment
open Leads.Shell.Commands.Forest.Utilities

let private handler forestHash =
    let deleteForestResult = deleteForestWorkflow forestHash
                               |> Reader.run deleteForestEnvironment

    match deleteForestResult with
    | Ok deletedForest ->
        deletedForest |> printSingleForest "Deleted Forest"
    | Error errorText ->
        errorText |> writeColoredLine ConsoleColor.Red

let appendForestDeleteSubCommand: SubCommandAppender =
    fun cmd ->        
        let deleteForestSubCommand =
            createCommandWithAlias "delete" "D" "The delete command deletes existing forest"
        let hashArgument =
            createArgument<string> "hash" "Provide the complete forest hash"   
        
        deleteForestSubCommand.AddArgument hashArgument
        
        deleteForestSubCommand.SetHandler(handler, hashArgument)
        
        cmd.AddCommand deleteForestSubCommand
        
        cmd      