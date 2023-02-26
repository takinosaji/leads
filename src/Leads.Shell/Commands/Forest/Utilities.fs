module Leads.Shell.Commands.Forest.Utilities

open Leads.Core.Forests
open Leads.Core.Forests.Forest.DTO

open Leads.SecondaryPorts.Forest.DTO

open Spectre.Console
open FSharp.Json

let private printValidForestTable tableTitle (validForests: ForestPODto list) =
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
                dto.Status,
                dto.LastModified.ToString(),
                dto.Created.ToString())
            ())
        validForests
        
    AnsiConsole.Write(table);

let printForests tableTitle validForests =
    printValidForestTable tableTitle validForests