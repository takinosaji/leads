module Leads.SecondaryAdapters.JsonFileBased.TrailAdapters

open System.IO
open Leads.SecondaryPorts.Config.DTO

open Leads.Utilities.Result

open Leads.SecondaryAdapters.JsonFileBased.ConfigAdapters.AllowedKeys

let private getTrailFolderPath
    (defaultWorkingDirPath: string)
    (validConfigurationDto: ValidConfigSIDto)
    streamHash = 
    let workingDirPath = ValidConfigSIDto.findOrDefault
                            validConfigurationDto
                            WorkingDirKey
                            defaultWorkingDirPath
    Path.Combine(workingDirPath, streamHash)
    

let private addTrail
    (workingDirPath: string)
    (trailToAdd: TrailSIDto) =
        result {
            return trailToAdd
        }
        // let forestsFilePath = Path.Combine(workingDirPath, forestFileName)
        //
        // result {
        //     let! forestsOption = listForests forestsFilePath
        //     let! _ =
        //         match forestsOption with
        //         | Some forests ->
        //             match List.tryFind (fun li -> li.Hash = forestToAdd.Hash) forests with
        //             | Some foundForest ->
        //                 Error $"The forest with Hash {foundForest.Hash} already exists"
        //             | None ->
        //                 forestToAdd.Hash
        //                 |> ensureForestFolderCreated workingDirPath
        //                 
        //                 [forestToAdd]                        
        //                 |> List.append forests  
        //                 |> persistForests forestsFilePath
        //         | None ->
        //             forestToAdd.Hash
        //             |> ensureForestFolderCreated workingDirPath
        //             
        //             [forestToAdd]
        //             |> persistForests forestsFilePath
        //                 
        //     return forestToAdd
        // }    
    
let createLocalJsonFileTrailAdapters defaultWorkingDirPath =
    {|
        addTrail = fun validConfigurationDto trailDto ->
           addTrail <| getTrailFolderPath defaultWorkingDirPath validConfigurationDto <| trailDto           
    |}