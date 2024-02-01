module Leads.Shell.Commands.Forest.Find

open System
open System.CommandLine

open Leads.Core.Forests
open Leads.Utilities.Dependencies

open Leads.Core.Forests.Workflows

open Leads.Shell
open Leads.Shell.Utilities
open Leads.Shell.Commands.Forest.Utilities
open Leads.Shell.Commands.Forest.Environment

let private handler searchText allOption activeOption completedOption archivedOption =
    reader {
        let statuses = ForestStatuses.composeStatuses allOption activeOption completedOption archivedOption
        
        let! findForestsResult = describeForestsWorkflow searchText statuses
        
        match findForestsResult with
        | Ok forestsOption ->
            match forestsOption with
            | None -> ()
            | Some forests ->
                forests |> printForests "Found Forests"
        | Error errorText ->
            errorText |> writeColoredLine ConsoleColor.Red
    } |> Reader.run findForestEnvironment
    
let appendForestFindSubCommand: SubCommandAppender =
    fun cmd ->        
        let findForestSubCommand =
            createCommandWithAlias "find" "fi" "The find command searches forests by name or hash"
        let searchTextArgument =
            createArgument<string> "searchText" "Provide the complete or partial forest hash or name"           
        let allOption =
            createOptionWithAlias<bool> "all" "A" "Include all forests in the search" false  
        let activeOption =
            createOption<bool>  "active" "Include only completed forests in the search" false
        let completedOption =
            createOption<bool>  "completed" "Include only completed forests in the search" false  
        let archivedOption =
            createOptionWithAlias<bool>  "archived" "r" "Include only archived forests in the search" false
        
        findForestSubCommand.AddArgument searchTextArgument
        findForestSubCommand.AddOption allOption
        findForestSubCommand.AddOption activeOption
        findForestSubCommand.AddOption completedOption
        findForestSubCommand.AddOption archivedOption
        
        findForestSubCommand.SetHandler(handler, searchTextArgument, allOption, activeOption, completedOption, archivedOption)
        
        cmd.AddCommand findForestSubCommand
        
        cmd      