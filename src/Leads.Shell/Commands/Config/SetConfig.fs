module Leads.Shell.Commands.Config.Set

open System
open System.CommandLine
open Leads.Shell
open Leads.Shell.Utilities

let private handler = fun k v ->
    Console.WriteLine "set"
    ()


let appendSetConfigSubCommand: SubCommandBinder =
    fun cmd ->    
        let getConfigSubCommand = Command("set", "The set command updates the specific configuration key with the value")   

        let keyArg = createArgument<string> "key" "Config Key"
        let valueArg = createArgument<string> "value" "Config Value"
        
        getConfigSubCommand.AddArgument keyArg
        getConfigSubCommand.AddArgument valueArg
                  
        getConfigSubCommand.SetHandler(handler, keyArg, valueArg)
        
        cmd.AddCommand getConfigSubCommand
        
        cmd      