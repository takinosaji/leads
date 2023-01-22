﻿module Leads.Shell.Commands.Config.List

open System
open System.CommandLine

open Leads.Core.Config
open Leads.Core.Config.Workflows
open Leads.Core.Utilities.Dependencies

open Leads.DrivenAdapters.ConfigAdapters

open Leads.Shell
open Leads.Shell.Utilities

let private printConfiguration (configurationDto: ConfigOutputDto) =
    match configurationDto with
    | Some configuration ->       
        let validItemsToPrint = List.choose (fun li -> match li with  | ValidEntryDto dto -> Some $"{dto.Key} = {dto.Value}" | _ -> None ) configuration
        match validItemsToPrint with
        | [_] ->
            List.iter (fun (li:string) -> Console.WriteLine li) validItemsToPrint
        | _ -> ()
        
        let validValuesToPrint = List.choose (fun li -> match li with | InvalidValueDto dto -> Some $"{dto.Key} = {dto.Value} | Error: ${dto.Error}" | _ -> None) configuration
        match validValuesToPrint with
        | [_] ->
            "Invalid values" |> writeColoredLine ConsoleColor.Red
            List.iter (fun (li:string) -> Console.WriteLine li) validValuesToPrint
        | _ -> ()
        
        let invalidValuesToPrint = List.choose (fun li -> match li with | InvalidKeyDto dto -> Some $"{dto.Key} | {dto.Error}" | _ -> None) configuration
        match invalidValuesToPrint with
        | [_] ->
            "Invalid keys" |> writeColoredLine ConsoleColor.Red
            List.iter (fun (li:string) -> Console.WriteLine li) invalidValuesToPrint
        | _ -> ()
    | None -> ()
        
let private handler = fun (_:unit) ->
    reader {        
        let! configurationResult = listConfigWorkflow()
        
        match configurationResult with
        | Ok configuration ->
            configuration |> printConfiguration
        | Error errorText ->
            errorText |> writeColoredLine ConsoleColor.Red
    } |> Reader.run {
        provideConfig = provideJsonFileConfiguration
        applyConfigValue = applyJsonFileConfiguration
    }
    
let appendListForestsSubCommand: SubCommandAppender =
    fun cmd ->    
        let getConfigSubCommand = Command("list", "The get command retrieves all config keys and values")   
                  
        getConfigSubCommand.SetHandler(handler)
        
        cmd.AddCommand getConfigSubCommand
        
        cmd      