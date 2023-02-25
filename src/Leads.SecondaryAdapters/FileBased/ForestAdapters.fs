module Leads.SecondaryAdapters.FileBased.ForestAdapters

open System.IO
open FSharp.Json

open Leads.Utilities.Result

open Leads.SecondaryPorts.Config.DTO
open Leads.SecondaryPorts.Forest.DTO

let private getForestFilePath
    (defaultWorkingDirPath: string)
    (validConfigurationDto: ValidConfigSIDto) = 
    let workingDirPath = ValidConfigSIDto.findOrDefault
                                 validConfigurationDto
                                 ConfigKeys.WorkingDirKey
                                 defaultWorkingDirPath
    Path.Combine(workingDirPath, "forests.json")  

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
    (forestsFilePath: string)
    (findCriteria: AdditiveFindCriteriaDto) =
        result {
            let! forestOption = listForests forestsFilePath 
            return
                match forestOption with
                | Some forests ->
                    let filteredForests =
                        forests                        
                        |> List.filter (forestFieldPredicate (fun f -> f.Name) findCriteria.Name) 
                        |> List.filter (forestFieldPredicate (fun f -> f.Hash) findCriteria.Hash)            
                        |> List.filter (fun li -> List.contains li.Status findCriteria.Statuses)
                    match filteredForests with
                    | [] -> None
                    | _ -> Some filteredForests
                | None ->
                    None
        }

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
    (forestsFilePath: string)
    (forestDto: ForestSIDto) =
        result {
            let! forestsOption = listForests forestsFilePath
            let! _ =
                match forestsOption with
                | Some forests ->
                    match List.choose (fun li ->
                        if li.Hash = forestDto.Hash  then
                            Some ("hash", li.Hash)
                        elif li.Name = forestDto.Name then
                            Some ("name", li.Name)
                        else
                            None
                            ) forests with
                    | firstFinding :: _ ->
                        Error $"The forest with {fst firstFinding} {snd firstFinding} already exists"
                    | [] ->
                        List.append forests [forestDto] 
                        |> persistForests forestsFilePath
                | None ->
                    persistForests forestsFilePath [forestDto]
                        
            return forestDto
        }
        
let updateForest
    (forestsFilePath: string)
    (forestToUpdate: ForestSIDto) =
    result {
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
                Error $"Forest with Hash={forestToUpdate.Hash} has not been found"                
    }
    
    
let createLocalJsonFileForestAdapters defaultWorkingDirPath =
    {|
        // getForest = fun validConfigurationDto forestNameOrHash ->
        //     getForestByNameOrHash <| getForestFilePath defaultWorkingDirPath validConfigurationDto <| forestNameOrHash
        addForest = fun validConfigurationDto forestDto ->
           addForest <| getForestFilePath defaultWorkingDirPath validConfigurationDto <| forestDto           
        findForests = fun validConfigurationDto searchCriteriaDto ->
           findForests <| getForestFilePath defaultWorkingDirPath validConfigurationDto <| searchCriteriaDto
        updateForest = fun validConfigurationDto forestDto ->
           updateForest <| getForestFilePath defaultWorkingDirPath validConfigurationDto <| forestDto
           
    |}