module Leads.Shell.Commands.Forest.List

open System

open System.CommandLine

open Leads.Core.Forests
open Leads.Core.Forests.Forest.DTO
open Leads.Utilities.Dependencies

open Leads.Core.Forests.Workflows

open Leads.Shell
open Leads.Shell.Utilities
open Leads.Shell.Commands.Forest.Utilities
open Leads.Shell.Commands.Forest.Environment
    
let printForests = function
   | Some (forestDTOs: ForestPODto list) ->
        let validForestsToPrint = List.choose (fun li -> match li with | ValidForestPODtoCase dto -> Some dto | _ -> None ) forestDTOs
        match validForestsToPrint with
        | [] -> ()
        | _ -> printValidForestTable validForestsToPrint
        
        let invalidValuesToPrint = List.choose (fun li -> match li with | InvalidForestPODtoCase dto -> Some dto | _ -> None) forestDTOs
        match invalidValuesToPrint with
        | [] -> ()
        | _ -> printInvalidForestTable invalidValuesToPrint
   | None -> ()
    
let private handler =
    fun allOption completedOption archivedOption ->
    reader {
        let statuses = ForestStatuses.composeStatuses allOption completedOption archivedOption
        
        let! forestsListResult = listForestsWorkflow statuses
        
        match forestsListResult with
        | Ok forests ->
            forests |> printForests
        | Error errorText ->
            errorText |> writeColoredLine ConsoleColor.Red
    } |> Reader.run findForestEnvironment
    
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
        
        listForestsSubCommand.AddOption allOption
        listForestsSubCommand.AddOption completedOption
        listForestsSubCommand.AddOption archivedOption
        
        listForestsSubCommand.SetHandler(handler, allOption, completedOption, archivedOption)
        
        cmd.AddCommand listForestsSubCommand
        
        cmd      