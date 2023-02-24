module Leads.Shell.Commands.Forest.Describe

open System
open System.CommandLine

open Leads.Core.Forests
open Leads.Utilities.Dependencies

open Leads.Core.Forests.Workflows

open Leads.Shell
open Leads.Shell.Utilities
open Leads.Shell.Commands.Forest.Utilities
open Leads.Shell.Commands.Forest.Environment
    

    
let private handler searchText allOption completedOption archivedOption =
    reader {
        let statuses = ForestStatuses.composeStatuses allOption completedOption archivedOption
        
        let! findForestResult = describeForestsWorkflow searchText statuses
        
        match findForestResult with
        | Ok forest ->
            forest |> printForests
        | Error errorText ->
            errorText |> writeColoredLine ConsoleColor.Red
    } |> Reader.run findForestEnvironment
    
let appendForestDescribeSubCommand: SubCommandAppender =
    fun cmd ->        
        let describeForestSubCommand =
            createCommand "describe" "The describe command retrieves searches forests by name or hash"
        let searchTextArgument =
            createArgument<string> "searchText" "Provide the complete or partial forest hash or name"           
        let allOption =
            createOptionWithAlias<bool> "all" "A" "Include all forests in the search" false  
        let completedOption =
            createOption<bool>  "completed" "Include only completed forests in the search" false  
        let archivedOption =
            createOption<bool>  "archived" "Include only archived forests in the search" false
        
        describeForestSubCommand.AddArgument searchTextArgument
        describeForestSubCommand.AddOption allOption
        describeForestSubCommand.AddOption completedOption
        describeForestSubCommand.AddOption archivedOption
        
        describeForestSubCommand.SetHandler(handler, searchTextArgument, allOption, completedOption, archivedOption)
        
        cmd.AddCommand describeForestSubCommand
        
        cmd      