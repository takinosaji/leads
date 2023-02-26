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
    fun allOption completedOption archivedOption ->
    reader {
        let statuses = ForestStatuses.composeStatuses allOption completedOption archivedOption
        
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