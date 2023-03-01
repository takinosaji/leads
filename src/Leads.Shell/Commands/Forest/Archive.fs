module Leads.Shell.Commands.Forest.Archive

open System

open System.CommandLine

open Leads.Core.Forests.Workflows
open Leads.Utilities.Dependencies

open Leads.Shell
open Leads.Shell.Utilities
open Leads.Shell.Commands.Forest.Environment
open Leads.Shell.Commands.Forest.Utilities

let private handler forestHash =
    let archiveForestResult = changeForestStatusWorkflow forestHash "active" "archived"
                               |> Reader.run updateForestEnvironment

    match archiveForestResult with
    | Ok archivedForest ->
        archivedForest |> printSingleForest "Archived Forest"
    | Error errorText ->
        errorText |> writeColoredLine ConsoleColor.Red

let appendForestArchiveSubCommand: SubCommandAppender =
    fun cmd ->        
        let archiveForestSubCommand =
            createCommandWithAlias "archive" "r" "The archive command completes existing active forest"
        let hashArgument =
            createArgument<string> "hash" "Provide the complete or partial forest hash or name"   
        
        archiveForestSubCommand.AddArgument hashArgument
        
        archiveForestSubCommand.SetHandler(handler, hashArgument)
        
        cmd.AddCommand archiveForestSubCommand
        
        cmd      