module Leads.Shell.Commands.Config.Appender

open System.CommandLine
open Leads.Shell

open Leads.Shell.Commands.Config.Get
open Leads.Shell.Commands.Config.Set

let appendConfigCommands: RootCommandBinder =
    fun cmd ->
        
        let configCommand = Command("config", "The config command allows you to manipulate with utility configuration")
        
        configCommand
            |> appendGetConfigSubCommand
            |> appendSetConfigSubCommand
            |> cmd.AddCommand
        
        cmd
    