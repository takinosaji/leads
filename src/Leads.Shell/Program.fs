module Leads.Shell.Startup

open System.CommandLine
open Leads.Shell.Commands.Config.Appender

[<EntryPoint>]
let main args =
    let rootCommand = RootCommand("Ultimate productivity and task management app.")
    
    rootCommand
        |> appendConfigCommands   
    
    rootCommand.Invoke(args)