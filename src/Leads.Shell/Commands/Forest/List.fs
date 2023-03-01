module Leads.Shell.Commands.Forest.List

open System

open System.CommandLine

open Leads.Core.Forests
open Leads.Utilities.Dependencies

open Leads.Core.Forests.Workflows

open Leads.Shell
open Leads.Shell.Utilities
open Leads.Shell.Commands.Forest.Utilities
open Leads.Shell.Commands.Forest.Environment
        
let private handler =
    fun allOption activeOption completedOption archivedOption ->
    reader {
        let statuses = ForestStatuses.composeStatuses allOption activeOption completedOption archivedOption
        
        let! forestsListResult = listForestsWorkflow statuses
        
        match forestsListResult with
        | Ok forestsOption ->
            match forestsOption with
            | None -> ()
            | Some forests ->
                forests |> printForests "Found Forests"
        | Error errorText ->
            errorText |> writeColoredLine ConsoleColor.Red
    } |> Reader.run findForestEnvironment
    
let appendForestListSubCommand: SubCommandAppender =
    fun cmd ->        
        let listForestsSubCommand =
            createCommand "list" "The get command retrieves the existing forests"
        let allOption =
            createOptionWithAlias<bool> "all" "A" "Show all forests" false  
        let activeOption =
            createOption<bool>  "active" "Include only completed forests in the search" false
        let completedOption =
            createOption<bool>  "completed" "Include only completed forests in the search" false  
        let archivedOption =
            createOptionWithAlias<bool>  "archived" "r" "Include only archived forests in the search" false
        
        listForestsSubCommand.AddOption allOption
        listForestsSubCommand.AddOption activeOption
        listForestsSubCommand.AddOption completedOption
        listForestsSubCommand.AddOption archivedOption
        
        listForestsSubCommand.SetHandler(handler, allOption, activeOption, completedOption, archivedOption)
        
        cmd.AddCommand listForestsSubCommand
        
        cmd      