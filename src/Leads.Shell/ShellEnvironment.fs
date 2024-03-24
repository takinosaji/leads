module Leads.Shell.ShellEnvironment

open System
open System.IO

open Leads.Shell.Utilities

let variables = {|
        appDirPath = $"{Environment.GetFolderPath Environment.SpecialFolder.UserProfile}/.leads"
    |}

let private ensureFolders dirPaths =
    try
        Seq.iter (fun dp ->
            dp
                |> Directory.CreateDirectory
                |> ignore
            ) dirPaths
    with
    | excp ->
        writeErrorLine $"Could not create application directories in the file system\nError:\n{excp.Message}"
  
let init () =
    ensureFolders (seq { yield variables.appDirPath })

