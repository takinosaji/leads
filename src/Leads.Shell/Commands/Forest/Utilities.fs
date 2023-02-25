module Leads.Shell.Commands.Forest.Utilities

open Leads.Core.Forests
open Leads.Core.Forests.Forest.DTO

open Leads.SecondaryPorts.Forest.DTO

open Spectre.Console
open FSharp.Json

let printValidForestTable (validForests: ValidForestPODto list) =
    let table = Table()

    table.AddColumn("Name")
    table.AddColumn("Hash")
    table.AddColumn("Status")
    table.AddColumn("LastModified")
    table.AddColumn("Created")
    
    table.Title = TableTitle("Valid Forests")
    
    List.iter
        (fun dto ->
            table.AddRow(
                dto.Name,
                dto.Hash,
                dto.Status,
                dto.LastModified.ToString(),
                dto.Created.ToString())
            ())
        validForests
        
    AnsiConsole.Write(table);

let printInvalidForestTable (invalidForests: InvalidForestOutputDto list) =
    let table = Table()

    table.AddColumn("Error")
    table.AddColumn("Forest")
    
    table.Title <- TableTitle("Invalid Forests")
    
    List.iter
        (fun dto ->
            table.AddRow(
                Json.serialize dto.Forest,
                "[red]{dto.Error}[/]")
            ())
        invalidForests
        
    AnsiConsole.Write(table);


  