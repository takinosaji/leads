﻿module Leads.Shell.Commands.Forest.Complete

open System

open System.CommandLine

open Leads.Core.Forests
open Leads.Core.Forests.Workflows
open Leads.SecondaryPorts.Forest
open Leads.Utilities.Dependencies

open Leads.Shell
open Leads.Shell.Utilities
open Leads.Shell.Commands.Forest.Environment
open Leads.Shell.Commands.Forest.Utilities

let private handler forestHash =
    let completeForestResult = changeForestStatusWorkflow forestHash ForestStatus.Active ForestStatus.Completed
                               |> Reader.run updateForestEnvironment

    match completeForestResult with
    | Ok completedForest ->
        completedForest |> printSingleForest "Completed Forest"
    | Error errorText ->
        errorText |> writeColoredLine ConsoleColor.Red
 
let appendForestCompleteSubCommand: SubCommandAppender =
    fun cmd ->        
        let completeForestSubCommand =
            createCommand "complete" "The complete command completes existing active forest"
        let hashArgument =
            createArgument<string> "hash" "Provide the complete forest hash"   
        
        completeForestSubCommand.AddArgument hashArgument
        
        completeForestSubCommand.SetHandler(handler, hashArgument)
        
        cmd.AddCommand completeForestSubCommand
        
        cmd      