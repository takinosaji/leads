module Leads.Shell.Commands.Forest.List

open System

open System.CommandLine

open Leads.Core.Forests.DTO
open Leads.Core.Forests.Workflows
open Leads.Core.Utilities.Dependencies

open Leads.DrivenAdapters.ForestAdapters

open Leads.Shell
open Leads.Shell.Utilities

let private printForests (forestsDto: ForestsDto) =
   ()

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
        | Ok configuration ->
            configuration |> printForests
        | Error errorText ->
            errorText |> writeColoredLine ConsoleColor.Red
    } |> Reader.run {
        provideForests = provideJsonFileForests
    }
    
let appendListForestsSubCommand: SubCommandAppender =
    fun cmd ->        
        let getConfigSubCommand =
            createCommand "list" "The get command retrieves the existing forests"
        let allOption =
            createOptionWithAlias<bool> "all" "Show all forests if additional options are not provided" "A" false  
        let completedOption =
            createOption<bool>  "archived" "Show only completed forests if additional options are not provided" false  
        let archivedOption =
            createOption<bool>  "archived" "Show only archived forests if additional options are not provided" false    
        
        getConfigSubCommand.SetHandler(handler, allOption, completedOption, archivedOption)
        
        cmd.AddCommand getConfigSubCommand
        
        cmd      