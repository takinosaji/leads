module Leads.SecondaryAdapters.JsonFileBased.ForestAdapters

open System.IO
open FSharp.Json

open Leads.Utilities.Result

open Leads.SecondaryPorts.Forest.DTO

open Leads.SecondaryAdapters.JsonFileBased.Utilities

let private forestFileName = "forests.json"

let private listForests
    (forestsFilePath: string)    
    : Result<ForestSODto list option, string> =         
        using (File.Open(forestsFilePath, FileMode.OpenOrCreate))
            (fun fileStream -> 
                use reader = new StreamReader(fileStream)
                let content = reader.ReadToEnd()
                match content with
                | "" -> Ok None
                | _ ->
                    try
                        let forests = Json.deserialize<ForestSODto list> content
                        match forests with
                        | [] -> Ok None
                        | _ -> Ok (Some forests)
                    with excp ->
                        Error(excp.Message)
                )
            
// let private getForestByNameOrHash
//     (forestsFilePath: string)
//     (forestNameOrHash: string)
//     : Result<ForestSODto option, string> =
//         result {
//             let! forestOption = listForests forestsFilePath
//             match forestOption with
//             | Some forests ->
//                 return List.tryFind (fun li -> li.Name = forestNameOrHash || li.Hash = forestNameOrHash) forests
//             | None ->
//                 return None
//         }
      
let private forestFieldPredicate (fieldResolver: ForestSODto -> string) textCriteria forest =
    let fieldValue = fieldResolver forest
    match textCriteria with
    | Any -> true
    | Contains textToSearch ->
        fieldValue.ToLower().Contains(textToSearch.ToLower())
    | Exact textToSearch ->
        fieldValue.ToLower() = textToSearch.ToLower()
let private findForests
    (workingDirPath: string)
    (orFindCriteria: OrFindCriteria) =
        let forestsFilePath = Path.Combine(workingDirPath, forestFileName)
        
        result {
            let! forestOption = listForests forestsFilePath 
            return
                match forestOption with
                | Some forests ->
                    let filteredForests =
                        orFindCriteria
                        |> List.map (fun andCriteria ->                              
                            forests                        
                            |> List.filter (forestFieldPredicate (fun f -> f.Name) andCriteria.Name) 
                            |> List.filter (forestFieldPredicate (fun f -> f.Hash) andCriteria.Hash)            
                            |> List.filter (fun li -> List.contains li.Status andCriteria.Statuses)
                        )
                        |> List.concat
                        |> List.distinct                    
                 
                    match filteredForests with
                    | [] -> None
                    | _ -> Some filteredForests
                | None ->
                    None
        }

let private ensureForestFolderCreated workingDirPath forestHash =
    try    
        let folderPath = Path.Combine(workingDirPath, forestHash)
        match Directory.Exists folderPath with
        | false ->
            Directory.CreateDirectory(folderPath) |> ignore
            Ok ()
        | true -> Ok ()
    with excp ->
        Error(excp.Message)

let private persistForests
    (forestsFilePath: string)
    (forestsDto: ForestSIDto list) =    
    try   
        let json = Json.serialize forestsDto    
        File.WriteAllText(forestsFilePath, json)
        Ok ()
    with excp ->
        Error(excp.Message)

let private addForest
    (workingDirPath: string)
    (forestToAdd: ForestSIDto) =
        let forestsFilePath = Path.Combine(workingDirPath, forestFileName)
        
        result {
            let! forestsOption = listForests forestsFilePath
            let! _ =
                match forestsOption with
                | Some forests ->
                    match List.tryFind (fun li -> li.Hash = forestToAdd.Hash) forests with
                    | Some foundForest ->
                        Error $"The forest with Hash {foundForest.Hash} already exists"
                    | None ->
                        forestToAdd.Hash
                        |> ensureForestFolderCreated workingDirPath
                        
                        [forestToAdd]                        
                        |> List.append forests  
                        |> persistForests forestsFilePath
                | None ->
                    forestToAdd.Hash
                    |> ensureForestFolderCreated workingDirPath
                    
                    [forestToAdd]
                    |> persistForests forestsFilePath
                        
            return forestToAdd
        }
        
let updateForest
    (workingDirPath: string)
    (forestToUpdate: ForestSIDto) =
    result {
        let forestsFilePath = Path.Combine(workingDirPath, forestFileName)
        let! forestsOption = listForests forestsFilePath
        
        return!
            match forestsOption with
            | Some forests ->
                let indexToReplace = List.findIndex (fun li -> li.Hash = forestToUpdate.Hash) forests
                     
                forests
                |> List.removeAt indexToReplace
                |> List.insertAt indexToReplace forestToUpdate
                |> persistForests forestsFilePath
            | None ->
                Error $"Forest with Hash {forestToUpdate.Hash} has not been found"                
    }
  
let private ensureForestFolderDeleted workingDirPath forestHash =
    try    
        let folderPath = Path.Combine(workingDirPath, forestHash)
        match Directory.Exists folderPath with
        | true ->
            Directory.Delete(folderPath)
            Ok ()
        | false -> Ok ()
    with excp ->
        Error(excp.Message)
        
let deleteForest
    (workingDirPath: string)
    (forestToDelete: ForestSIDto) =
    result {
        let forestsFilePath = Path.Combine(workingDirPath, forestFileName)
        let! forestsOption = listForests forestsFilePath
        
        return!
            match forestsOption with
            | Some forests ->
                forests
                |> List.where (fun li -> li.Hash <> forestToDelete.Hash) 
                |> persistForests forestsFilePath
                |> ignore
                
                forestToDelete.Hash
                |> ensureForestFolderDeleted workingDirPath
            | None ->
                Error $"Forest with Hash {forestToDelete.Hash} has not been found"                
    }
       
let createLocalJsonFileForestAdapters defaultWorkingDirPath =
    {|
        addForest = fun validConfigurationDto forestDto ->
           addForest <| getWorkingDirPath defaultWorkingDirPath validConfigurationDto <| forestDto           
        findForests = fun validConfigurationDto searchCriteriaDto ->
           findForests <| getWorkingDirPath defaultWorkingDirPath validConfigurationDto <| searchCriteriaDto
        updateForest = fun validConfigurationDto forestDto ->
           updateForest <| getWorkingDirPath defaultWorkingDirPath validConfigurationDto <| forestDto
        deleteForest = fun validConfigurationDto forestDto ->
           deleteForest <| getWorkingDirPath defaultWorkingDirPath validConfigurationDto <| forestDto
    |}