module Leads.DrivenAdapters.ForestAdapters

open System.IO
open FSharp.Json
open Leads.Core.Forests.DTO
open Leads.Core.Forests.Workflows
open Leads.Core.Utilities.ConstrainedTypes

let private ForestsFileName = "forests.json"

let provideJsonFileForests: ForestsProvider =
    fun workingDirPath ->
        let forestsFilePath = Path.Combine(workingDirPath, ForestsFileName)
        
        using (File.Open(forestsFilePath, FileMode.OpenOrCreate))
            (fun fileStream -> 
                use reader = new StreamReader(fileStream)
                let content = reader.ReadToEnd()
                match content with
                | "" -> Ok None
                | _ ->
                    try
                        Ok(Some(Json.deserialize<ForestDrivenDto list> content))
                    with excp ->
                        Error(ErrorText excp.Message)
            )