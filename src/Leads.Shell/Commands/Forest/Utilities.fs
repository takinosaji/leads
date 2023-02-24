module Leads.Shell.Commands.Forest.Utilities

open Leads.Core.Forests
open Leads.Core.Forests.ForestDTO

open Leads.SecondaryPorts.Forest.DTO

open Spectre.Console
open FSharp.Json

let printValidForestTable (validForests: ValidForestOutputDto list) =
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

let printForests = function
   | Some (forestDTOs: ForestPrimaryOutputDto list) ->
        let validForestsToPrint = List.choose (fun li -> match li with | ValidForestOutputDto dto -> Some dto | _ -> None ) forestDTOs
        match validForestsToPrint with
        | [] -> ()
        | _ -> printValidForestTable validForestsToPrint
        
        let invalidValuesToPrint = List.choose (fun li -> match li with | InvalidForestOutputDto dto -> Some dto | _ -> None) forestDTOs
        match invalidValuesToPrint with
        | [] -> ()
        | _ -> printInvalidForestTable invalidValuesToPrint
   | None -> ()
  