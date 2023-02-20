module Leads.Shell.Commands.Forest.Describe

open System

open System.CommandLine

open Leads.Core.Forests.ForestDto
open Leads.Utilities.Dependencies

open Leads.DrivenPorts.Forest.DTO

open Leads.Shell
open Leads.Shell.Utilities
open Leads.Shell.Commands.Forest.Environment

open Spectre.Console

let private printForest (forestDto: ValidForestDto) =
    
    let table = Table()
    
    table.Title <- TableTitle("Forest")

    table.AddColumn("Field")
    table.AddColumn("Value")
    
    table.AddRow(nameof(forestDto.Hash), forestDto.Hash)
    table.AddRow(nameof(forestDto.Name), forestDto.Name)
    table.AddRow(nameof(forestDto.Status), forestDto.Status)
    table.AddRow(nameof(forestDto.Created), forestDto.Created.ToString())
    table.AddRow(nameof(forestDto.LastModified), forestDto.LastModified.ToString())
           
    AnsiConsole.Write(table);
    
let private handler name =
    reader {       
        let! describeForestResult = describeForestWorkflow name
        
        match describeForestResult with
        | Ok forest ->
            forest |> printForest
        | Error errorText ->
            errorText |> writeColoredLine ConsoleColor.Red
    } |> Reader.run completeForestEnvironment
    
let appendForestDescribeSubCommand: SubCommandAppender =
    fun cmd ->        
        let completeForestSubCommand =
            createCommand "describe" "The describe command retrieves all fields of existing forests to display"
        let searchTextArgument =
            createArgument<string> "searchText" "Provide the complete or partial forest hash or name"   
        
        completeForestSubCommand.AddArgument searchTextArgument
        
        completeForestSubCommand.SetHandler(handler, searchTextArgument)
        
        cmd.AddCommand completeForestSubCommand
        
        cmd      