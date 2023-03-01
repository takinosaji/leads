module Leads.Shell.Commands.Config.Appender

open Leads.Shell
open Leads.Shell.Utilities

open Leads.Shell.Commands.Config.Get
open Leads.Shell.Commands.Config.Set
open Leads.Shell.Commands.Config.List

let appendConfigCommands: RootCommandAppender =
    fun cmd ->       
        let configCommand =
            createCommand "config" "The config command allows you to manipulate with utility configuration"
        
        configCommand
            |> appendConfigGetSubCommand
            |> appendConfigSetSubCommand
            |> appendConfigListSubCommand
            |> cmd.AddCommand
        
        cmd
    