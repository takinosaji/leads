module Leads.Shell.Commands.Forest.Appender

open Leads.Shell
open Leads.Shell.Utilities

open Leads.Shell.Commands.Forest.List

let appendForestCommands: RootCommandAppender =
    fun cmd ->   
        let forestCommand =
            createCommand "forest" "The stream command allows you to manipulate with the streams"
        
        forestCommand
            |> appendListForestsSubCommand
            |> cmd.AddCommand
        
        cmd
    