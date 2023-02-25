module Leads.Shell.Commands.Forest.Complete

open System

open System.CommandLine

open Leads.Core.Forests
open Leads.Core.Forests.Workflows
open Leads.Core.Forests.Forest.DTO
open Leads.Utilities.Dependencies

open Leads.SecondaryPorts.Forest.DTO

open Leads.Shell
open Leads.Shell.Utilities
open Leads.Shell.Commands.Forest.Environment
open Spectre.Console

let private printCompletedForest (forestDto: ValidForestOutputDto) =
    
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
    
let private handler searchText =
    reader {
        let! forests = describeForestsWorkflow searchText ForestStatuses.Active
                       |> Reader.withEnv toFindForestsEnvironment
        
        match forests with
        | Ok None ->
            writeLine $"Any forests with name or id containing {searchText} have not been found"
        | Ok (Some [forest]) ->
            let! completeForestResult = completeForestWorkflow forest
        
            match completeForestResult with
            | Ok forest ->
                forest |> printCompletedForest
            | Error errorText ->
                errorText |> writeColoredLine ConsoleColor.Red
            | Error errorText ->
            errorText |> writeColoredLine ConsoleColor.Red
    } |> Reader.run updateForestEnvironment
    
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