module Leads.Shell.Commands.Config.Get

open System
open System.CommandLine
open Leads.Shell
open Leads.Shell.Utilities

let private handler = fun k ->
    Console.WriteLine "get"
    ()


let appendGetConfigSubCommand: SubCommandBinder =
    fun cmd ->    
        let getConfigSubCommand = Command("get", "The get command retrieves the specific configuration value by key")   

        let keyArg = createArgument<string> "key" "Config Key"
        
        getConfigSubCommand.AddArgument keyArg
                  
        getConfigSubCommand.SetHandler(handler, keyArg)
        
        cmd.AddCommand getConfigSubCommand
        
        cmd      