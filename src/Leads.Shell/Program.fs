module Leads.Shell.Startup

open System.CommandLine
open System.IO

open Leads.Shell.Environment
open Leads.Shell.Utilities

open Leads.Shell.Commands.Config.Appender
open Leads.Shell.Commands.Forest.Appender

let ensureFolders dirPaths =
    try
        Seq.iter (fun dp ->
            dp
                |> Directory.CreateDirectory
                |> ignore
            ) dirPaths
    with
    | excp ->
        writeErrorLine $"Could not create application directories in the file system\nError:\n{excp.Message}"
    

[<EntryPoint>]
let main args =    
    ensureFolders (seq { yield shellEnvironment.defaultWorkingDirPath })
    
    let rootCommand = RootCommand("Ultimate productivity and task management app.")
    
    rootCommand
        |> appendConfigCommands
        |> appendForestCommands
        |> ignore
        
    rootCommand.Invoke(args)