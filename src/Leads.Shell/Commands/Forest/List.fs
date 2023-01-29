module Leads.Shell.Commands.Forest.List

open System

open System.CommandLine

open Leads.Core.Utilities.Dependencies
open Leads.Core.Utilities.ListExtensions

open Leads.Core.Forests.DTO
open Leads.Core.Forests.ForestStatus.DTO
open Leads.Core.Forests.Workflows

open Leads.DrivenAdapters.ConsoleAdapters

open Leads.Shell
open Leads.Shell.Utilities
open Leads.Shell.Commands.Forest.Environment


let private printValidForest (ValidForestDto validForestDto) =
    Console.WriteLine $"{nameof(validForestDto.Name)}: {validForestDto.Name}"
    Console.WriteLine $"{nameof(validForestDto.Hash)}: {validForestDto.Hash}"
    Console.WriteLine $"{nameof(validForestDto.Status)}: {validForestDto.Status}"
    Console.WriteLine $"{nameof(validForestDto.LastModified)}: {validForestDto.LastModified}"
    Console.WriteLine $"{nameof(validForestDto.Created)}: {validForestDto.Created}"

let private printInvalidForest (InvalidForestDto invalidForestDto) =
    Console.WriteLine $"{nameof(invalidForestDto.Error)}: {invalidForestDto.Error}"
    Console.WriteLine $"{nameof(invalidForestDto.Forest)}: {JSONize invalidForestDto.Forest}"

let private printForests = function
   | Some (forestDTOs: ForestOutboundDto list) ->
        let validForestsToPrint = List.choose (fun li -> match li with | ValidForestDto dto -> Some (ValidForestDto dto) | _ -> None ) forestDTOs
        match validForestsToPrint with
        | [_] ->
            List.iterp
                (fun validForestDto -> printValidForest validForestDto)
                (fun _ -> writeEmptyLine())
                validForestsToPrint
        | _ -> ()
        
        let invalidValuesToPrint = List.choose (fun li -> match li with | InvalidForestDto dto -> Some (InvalidForestDto dto) | _ -> None) forestDTOs
        match invalidValuesToPrint with
        | [_] ->
            "Invalid Forests" |> writeColoredLine ConsoleColor.Red
            List.iterp
                (fun invalidForestToPrint -> printInvalidForest invalidForestToPrint)
                (fun _ -> writeEmptyLine())
                invalidValuesToPrint
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
    } |> Reader.run environment
    
let appendListForestsSubCommand: SubCommandAppender =
    fun cmd ->        
        let getConfigSubCommand =
            createCommand "list" "The get command retrieves the existing forests"
        let allOption =
            createOptionWithAlias<bool> "all" "A" "Show all forests if additional options are not provided" false  
        let completedOption =
            createOption<bool>  "archived" "Show only completed forests if additional options are not provided" false  
        let archivedOption =
            createOption<bool>  "archived" "Show only archived forests if additional options are not provided" false    
        
        getConfigSubCommand.SetHandler(handler, allOption, completedOption, archivedOption)
        
        cmd.AddCommand getConfigSubCommand
        
        cmd      