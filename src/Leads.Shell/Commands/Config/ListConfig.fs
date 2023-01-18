module Leads.Shell.Commands.Config.List

open System
open System.CommandLine

open Leads.Core.Config
open Leads.Core.Config.Workflows
open Leads.Core.Utilities.ConstrainedTypes
open Leads.Core.Utilities.Dependencies

open Leads.DrivenAdapters.ConfigAdapters

open Leads.Shell
open Leads.Shell.Utilities

let private printConfiguration (configuration: ConfigOutput) =
    match configuration with
    | Some  ->
        let validEntries = List.filter (fun li -> match li with | ValidEntry _-> true) configuration
        let invalidValues = List.filter (fun li -> match li with | InvalidValue _-> true) configuration
        let invalidKeys = List.filter (fun li -> match li with | InvalidKey _-> true) configuration
        
        List.iter (fun li ->
            let printItem = $"{li.Key} = {li.Value} 
            Console.WriteLine li) validEntries
    | None -> ()
        
let private handler = fun (_:unit) ->
    reader {        
        let! configuration = listConfigWorkflow()
        configuration |> printConfiguration      
    } |> Reader.run {
        provideConfig = provideJsonFileConfiguration
        applyConfigValue = applyJsonFileConfiguration
    }
    
let appendListConfigSubCommand: SubCommandAppender =
    fun cmd ->    
        let getConfigSubCommand = Command("list", "The get command retrieves all config keys and values")   
                  
        getConfigSubCommand.SetHandler(handler)
        
        cmd.AddCommand getConfigSubCommand
        
        cmd      