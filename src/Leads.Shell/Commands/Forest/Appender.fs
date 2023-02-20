module Leads.Shell.Commands.Forest.Appender

open Leads.Shell
open Leads.Shell.Utilities

open Leads.Shell.Commands.Forest.List
open Leads.Shell.Commands.Forest.Add
open Leads.Shell.Commands.Forest.Describe

let appendForestCommands: RootCommandAppender =
    fun cmd ->   
        let forestCommand =
            createCommand "forest" "The forest command allows you to manipulate with the streams"
        
        forestCommand
            |> appendForestListSubCommand
            |> appendForestAddSubCommand
            |> appendForestDescribeSubCommand
            |> cmd.AddCommand
        
        cmd
    