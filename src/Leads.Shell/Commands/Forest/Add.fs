module Leads.Shell.Commands.Forest.Add

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


let private printForest forestDto =
    ()
    
let private handler name =
    reader {       
        let! forestsListResult = addForestWorkflow name
        
        match forestsListResult with
        | Ok forests ->
            forests |> printForests
        | Error errorText ->
            errorText |> writeColoredLine ConsoleColor.Red
    } |> Reader.run environment
    
let appendForestAddSubCommand: SubCommandAppender =
    fun cmd ->        
        let addForestSubCommand =
            createCommand "add" "The add command creates the new forest"
        let nameArgument =
            createArgument<string> "name" "Set the unique forest name"   
        
        addForestSubCommand.SetHandler(handler, nameArgument)
        
        cmd.AddCommand addForestSubCommand
        
        cmd      