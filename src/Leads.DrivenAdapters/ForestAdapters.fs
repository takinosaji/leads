module Leads.DrivenAdapters.ForestAdapters

open System
open System.IO
open FSharp.Json

open Leads.DrivenPorts.Forest.DTO

let private ForestsFileName = "forests.json"

let private provideJsonFileForests =
    fun workingDirPath ->
        
        //         let! getWorkingDirPathResult = getConfigValue WorkingDirKey
        //                             |> Reader.withEnv toGetConfigEnvironment
        // return result {
        //     let! workingDirPathOption = getWorkingDirPathResult
        //     let workingDirPath = ConfigValue.valueOrDefaultOption workingDirPathOption environment.defaultWorkingDirPath
        
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
                        Error(excp.Message)
            )
            
let private addForestToJsonFile =
    fun workingDirPath (forest: ForestDrivenDto) ->
        provideJsonFileForests workingDirPath
        Ok { Hash = "string"; Name = "string"; Created = DateTime.Now; LastModified = DateTime.Now; Status = "string" }
        
let createLocalJsonFileForestAdapters defaultWorkingDirPath =
    {|
       provideJsonFileForests = fun (_:unit) -> provideJsonFileForests defaultWorkingDirPath
       addForestToJsonFile = fun (forest: ForestDrivenDto) -> addForestToJsonFile defaultWorkingDirPath forest
    |}