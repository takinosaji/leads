﻿module Leads.Shell.Commands.Forest.Utilities

open System

open Leads.Core.Forests.Forest.DTO
open Leads.SecondaryPorts.Forest.DTO

open Spectre.Console

let printForests tableTitle validForests =
    let table = Table()

    table.AddColumn("Name")
    table.AddColumn("Hash")
    table.AddColumn("Status")
    table.AddColumn("LastModified")
    table.AddColumn("Created")
    
    table.Title = TableTitle(tableTitle)
    
    List.iter
        (fun dto ->
            table.AddRow(
                dto.Name,
                dto.Hash,
                dto.Status.ToString(),
                dto.UpdatedAt.ToString(),
                dto.CreatedAt.ToString())
            ())
        validForests
        
    AnsiConsole.Write(table)
    

let private injectColors coloredSnippets text  =
    List.fold
        (fun (s: String) coloredSnippets ->
            let coloredText = fst coloredSnippets
            let color = snd coloredSnippets
            s.Replace(coloredText, $"[{color}]{coloredText}[/]")
        ) text coloredSnippets
        
        
let printForestsWithColoredText tableTitle coloredSnippets validForests =   
    let table = Table()

    table.AddColumn("Name")
    table.AddColumn("Hash")
    table.AddColumn("Status")
    table.AddColumn("LastModified")
    table.AddColumn("Created")
    
    table.Title = TableTitle(tableTitle)
    
    List.iter
        (fun dto ->
            table.AddRow(
                dto.Name |> injectColors coloredSnippets,
                dto.Hash |> injectColors coloredSnippets,
                dto.Status.ToString(),
                dto.UpdatedAt.ToString(),
                dto.CreatedAt.ToString())
            ())
        validForests
        
    AnsiConsole.Write(table)
    
let printSingleForest tableTitle (forestDto: ForestPODto) =
    let table = Table()
    
    table.Title <- TableTitle(tableTitle)

    table.AddColumn("Field")
    table.AddColumn("Value")
    
    table.AddRow(nameof(forestDto.Hash), forestDto.Hash)
    table.AddRow(nameof(forestDto.Name), forestDto.Name)
    table.AddRow(nameof(forestDto.Status), forestDto.Status.ToString())
    table.AddRow(nameof(forestDto.CreatedAt), forestDto.CreatedAt.ToString())
    table.AddRow(nameof(forestDto.UpdatedAt), forestDto.UpdatedAt.ToString())

    AnsiConsole.Write(table);