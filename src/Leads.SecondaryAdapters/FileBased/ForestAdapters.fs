module Leads.SecondaryAdapters.FileBased.ForestAdapters

open System.IO
open FSharp.Json

open Leads.Utilities.Result

open Leads.SecondaryPorts.Config.DTO
open Leads.SecondaryPorts.Forest.DTO

let private getForestFilePath
    (defaultWorkingDirPath: string)
    (validConfigurationDto: ValidConfigSecondaryInputDto) = 
    let workingDirPath = ValidConfigSecondaryInputDto.findOrDefault
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
            
let private getForestByNameOrHash
    (forestsFilePath: string)
    (forestNameOrHash: string)
    : Result<ForestSODto option, string> =
        result {
            let! forestOption = listForests forestsFilePath
            match forestOption with
            | Some forests ->
                return List.tryFind (fun li -> li.Name = forestNameOrHash || li.Hash = forestNameOrHash) forests
            | None ->
                return None
        }
      
let private findForests
    (forestsFilePath: string)
    (findCriteria: FindCriteriaDto) =
        result {
            let! forestOption = listForests forestsFilePath 
            return
                match forestOption with
                | Some forests ->
                    let filteredForests =
                        forests
                        |> List.filter (fun li ->
                            match findCriteria.text with
                            | All -> true
                            | ContainsText textToSearch ->
                                li.Hash.ToLower().Contains(textToSearch.ToLower()) ||
                                li.Name.ToLower().Contains(textToSearch.ToLower()))         
                        |> List.filter (fun li -> List.contains li.Status findCriteria.statuses)
                    match filteredForests with
                    | [] -> None
                    | _ -> Some filteredForests
                | None ->
                    None
        }

let private persistForests
    (forestsFilePath: string)
    (forestsDto: ForestSecondaryInputDto list) =    
    try   
        let json = Json.serialize forestsDto    
        File.WriteAllText(forestsFilePath, json)
        Ok ()
    with excp ->
        Error(excp.Message)

let private addForest
    (forestsFilePath: string)
    (forestDto: ForestSecondaryInputDto) =
        result {
            let! forestOption = listForests forestsFilePath
            let! _ =
                match forestOption with
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
                        Error($"The forest with {fst firstFinding} {snd firstFinding} already exists")
                    | [] ->
                        List.append forests [forestDto] 
                        |> persistForests forestsFilePath
                | None ->
                    persistForests forestsFilePath [forestDto]
                        
            return forestDto
        }
        
let createLocalJsonFileForestAdapters defaultWorkingDirPath =
    {|
        getForest = fun validConfigurationDto forestNameOrHashDto ->
            getForestByNameOrHash <| getForestFilePath defaultWorkingDirPath validConfigurationDto <| forestNameOrHashDto
        addForest = fun validConfigurationDto forestDto ->
           addForest <| getForestFilePath defaultWorkingDirPath validConfigurationDto <| forestDto           
        findForests = fun validConfigurationDto searchCriteriaDto ->
           findForests <| getForestFilePath defaultWorkingDirPath validConfigurationDto <| searchCriteriaDto
        updateForest = fun validConfigurationDto forestDto ->
           updateForest <| getForestFilePath defaultWorkingDirPath validConfigurationDto <| forestDto
           
    |}