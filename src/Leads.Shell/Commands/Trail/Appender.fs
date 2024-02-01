module Leads.Shell.Commands.Trail.Appender

open Leads.Shell
open Leads.Shell.Utilities

open Leads.Shell.Commands.Trail.Add

let appendForestCommands: RootCommandAppender =
    fun cmd ->   
        let forestCommand =
            createCommand "trail" "The trail command allows you to manipulate with the trails"
        
        forestCommand
            |> appendTrailAddSubCommand
            |> cmd.AddCommand
        
        cmd
    