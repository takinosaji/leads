module Leads.Shell.Commands.Forest.Appender

open Leads.Shell
open Leads.Shell.Utilities

open Leads.Shell.Commands.Forest.List
open Leads.Shell.Commands.Forest.Add
open Leads.Shell.Commands.Forest.Find
open Leads.Shell.Commands.Forest.Complete
open Leads.Shell.Commands.Forest.Archive
open Leads.Shell.Commands.Forest.Delete
open Leads.Shell.Commands.Forest.Use

let appendForestCommands: RootCommandAppender =
    fun cmd ->   
        let forestCommand =
            createCommand "forest" "The forest command allows you to manipulate with the forests"
        
        forestCommand
            |> appendForestListSubCommand
            |> appendForestAddSubCommand
            |> appendForestFindSubCommand
            //|> appendForestDescribeSubCommand
            |> appendForestCompleteSubCommand
            |> appendForestArchiveSubCommand
            |> appendForestDeleteSubCommand
            |> appendForestUseSubCommand
            |> cmd.AddCommand
        
        cmd
    