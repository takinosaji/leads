module Leads.Shell.Commands.Forest.Complete

open System

open System.CommandLine

open Leads.Core.Forests.Forest.DTO
open Leads.Core.Forests.Workflows
open Leads.Utilities.Dependencies

open Leads.SecondaryPorts.Forest.DTO

open Leads.Shell
open Leads.Shell.Utilities
open Leads.Shell.Commands.Forest.Environment
open Spectre.Console

let private printCompletedForest (forestDto: ForestPODto) =
    
    let table = Table()
    
    table.Title <- TableTitle("Completed Forest")

    table.AddColumn("Field")
    table.AddColumn("Value")
    
    table.AddRow(nameof(forestDto.Hash), forestDto.Hash)
    table.AddRow(nameof(forestDto.Name), forestDto.Name)
    table.AddRow(nameof(forestDto.Status), forestDto.Status)
    table.AddRow(nameof(forestDto.Created), forestDto.Created.ToString())
    table.AddRow(nameof(forestDto.LastModified), forestDto.LastModified.ToString())
           
    AnsiConsole.Write(table);
    
let private handler forestHash =
    let completeForestResult = completeForestWorkflow forestHash
                               |> Reader.run updateForestEnvironment

    match completeForestResult with
    | Ok completedForest ->
        completedForest |> printCompletedForest
    | Error errorText ->
        errorText |> writeColoredLine ConsoleColor.Red

    
let appendForestCompleteSubCommand: SubCommandAppender =
    fun cmd ->        
        let completeForestSubCommand =
            createCommand "complete" "The complete command completes existing active forest"
        let nameOrHashArgument =
            createArgument<string> "searchText" "Provide the complete or partial forest hash or name"   
        
        completeForestSubCommand.AddArgument nameOrHashArgument
        
        completeForestSubCommand.SetHandler(handler, nameOrHashArgument)
        
        cmd.AddCommand completeForestSubCommand
        
        cmd      