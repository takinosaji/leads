﻿module Leads.Shell.Commands.Forest.Add

open System

open System.CommandLine

open Leads.Core.Forests.Forest.DTO
open Leads.Utilities.Dependencies

open Leads.SecondaryPorts.Forest.DTO
open Leads.Core.Forests.Workflows

open Leads.Shell
open Leads.Shell.Utilities
open Leads.Shell.Commands.Forest.Environment
open Spectre.Console

let private printAddedForest (forestDto: ForestPODto) =
    let table = Table()
    
    table.Title <- TableTitle("New Forest")

    table.AddColumn("Field")
    table.AddColumn("Value")
    
    table.AddRow(nameof(forestDto.Hash), forestDto.Hash)
    table.AddRow(nameof(forestDto.Name), forestDto.Name)
    table.AddRow(nameof(forestDto.Status), forestDto.Status.ToString())
    table.AddRow(nameof(forestDto.CreatedAt), forestDto.CreatedAt.ToString())
    table.AddRow(nameof(forestDto.UpdatedAt), forestDto.UpdatedAt.ToString())
           
    AnsiConsole.Write(table);
    
let private handler name =
    let addForestResult = addForestWorkflow name
                          |> Reader.run addForestEnvironment
    
    match addForestResult with
    | Ok forest ->
        forest |> printAddedForest
    | Error errorText ->
        errorText |> writeColoredLine ConsoleColor.Red
    
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