module Leads.Shell.Commands.Forest.Add

open System

open System.CommandLine

open Leads.Core.Forests.ForestDTO
open Leads.Utilities.Dependencies

open Leads.SecondaryPorts.Forest.DTO
open Leads.Core.Forests.Workflows

open Leads.Shell
open Leads.Shell.Utilities
open Leads.Shell.Commands.Forest.Environment
open Spectre.Console

let private printAddedForest (forestDto: ValidForestOutputDto) =
    
    let table = Table()
    
    table.Title <- TableTitle("New Forest")

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
        let! addForestResult = addForestWorkflow name
        
        match addForestResult with
        | Ok forest ->
            forest |> printAddedForest
        | Error errorText ->
            errorText |> writeColoredLine ConsoleColor.Red
    } |> Reader.run addForestEnvironment
    
let appendForestAddSubCommand: SubCommandAppender =
    fun cmd ->        
        let addForestSubCommand =
            createCommand "add" "The add command creates the new forest"
        let nameArgument =
            createArgument<string> "name" "Set the unique forest name"   
        
        addForestSubCommand.AddArgument nameArgument
        
        addForestSubCommand.SetHandler(handler, nameArgument)
        
        cmd.AddCommand addForestSubCommand
        
        cmd      