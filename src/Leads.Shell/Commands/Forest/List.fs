module Leads.Shell.Commands.Forest.List

open System

open System.CommandLine

open Leads.Utilities.Dependencies
open Leads.Utilities.ListExtensions

open Leads.DrivenPorts.Forest.DTO
open Leads.Core.Forests.ForestStatus.DTO
open Leads.Core.Forests.Workflows

open Leads.DrivenAdapters.ConsoleAdapters

open Leads.Shell
open Leads.Shell.Utilities
open Leads.Shell.Commands.Forest.Environment
open Spectre.Console


let printValidForestTable (validForests: ValidForestDto list) =
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

let printInvalidForestTable (invalidForests: InvalidForestDto list) =
    let table = Table()

    table.AddColumn("Error")
    table.AddColumn("Forest")
    
    table.Title <- TableTitle("Invalid Forests")
    
    List.iter
        (fun dto ->
            table.AddRow(
                JSONize dto.Forest,
                "[red]{dto.Error}[/]")
            ())
        invalidForests
        
    AnsiConsole.Write(table);

let private printForests = function
   | Some (forestDTOs: ForestDrivingDto list) ->
        let validForestsToPrint = List.choose (fun li -> match li with | ValidForestDto dto -> Some dto | _ -> None ) forestDTOs
        match validForestsToPrint with
        | [_] -> printValidForestTable validForestsToPrint
        | _ -> ()
        
        let invalidValuesToPrint = List.choose (fun li -> match li with | InvalidForestDto dto -> Some dto | _ -> None) forestDTOs
        match invalidValuesToPrint with
        | [_] -> printInvalidForestTable invalidValuesToPrint
        | _ -> ()
   | None -> ()

let private composeStatusDto =
    fun allOption completedOption archivedOption ->
    match allOption, completedOption, archivedOption with
    | true, _, _ -> ForestStatusDto.All
    | false, true, true -> ForestStatusDto.Completed ||| ForestStatusDto.Archived
    | false, true, false -> ForestStatusDto.Completed
    | false, false, true -> ForestStatusDto.Archived
    | _ -> ForestStatusDto.Active
    
let private handler =
    fun allOption completedOption archivedOption ->
    reader {
        let status = composeStatusDto allOption completedOption archivedOption
        
        let! forestsListResult = listForestsWorkflow status
        
        match forestsListResult with
        | Ok forests ->
            forests |> printForests
        | Error errorText ->
            errorText |> writeColoredLine ConsoleColor.Red
    } |> Reader.run getForestsEnvironment
    
let appendForestListSubCommand: SubCommandAppender =
    fun cmd ->        
        let listForestsSubCommand =
            createCommand "list" "The get command retrieves the existing forests"
        let allOption =
            createOptionWithAlias<bool> "all" "A" "Show all forests" false  
        let completedOption =
            createOption<bool>  "completed" "Show only completed forests if additional options are not provided" false  
        let archivedOption =
            createOption<bool>  "archived" "Show only archived forests if additional options are not provided" false    
        
        listForestsSubCommand.SetHandler(handler, allOption, completedOption, archivedOption)
        
        cmd.AddCommand listForestsSubCommand
        
        cmd      