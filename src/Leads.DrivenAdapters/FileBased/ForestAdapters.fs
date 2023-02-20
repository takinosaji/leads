module Leads.DrivenAdapters.FileBased.ForestAdapters

open System.IO
open FSharp.Json

open Leads.Utilities.Result

open Leads.DrivenPorts.Config.DTO
open Leads.DrivenPorts.Forest.DTO

let private getForestFilePath
    (defaultWorkingDirPath: string)
    (validConfigurationDto: ValidConfigDrivenInputDto) = 
    let workingDirPath = ValidConfigDrivenInputDto.findOrDefault
                                 validConfigurationDto
                                 ConfigKeys.WorkingDirKey
                                 defaultWorkingDirPath
    Path.Combine(workingDirPath, "forests.json")  

let private provideForests
    (forestsFilePath: string)
    : Result<ForestDrivenOutputDto list option, string> =         
        using (File.Open(forestsFilePath, FileMode.OpenOrCreate))
            (fun fileStream -> 
                use reader = new StreamReader(fileStream)
                let content = reader.ReadToEnd()
                match content with
                | "" -> Ok None
                | _ ->
                    try
                        Ok(Some(Json.deserialize<ForestDrivenOutputDto list> content))
                    with excp ->
                        Error(excp.Message)
                )

let private getForestByNameOrHash
    (forestsFilePath: string)
    (forestNameOrHash: string)
    : Result<ForestDrivenOutputDto option, string> =
        result {
            let! forestOption = provideForests forestsFilePath
            match forestOption with
            | Some forests ->
                return List.tryFind (fun li -> li.Name = forestNameOrHash || li.Hash = forestNameOrHash) forests
            | None ->
                return None
        }

let private persistForests
    (forestsFilePath: string)
    (forestsDto: ForestDrivenInputDto list) =    
    try   
        let json = Json.serialize forestsDto    
        File.WriteAllText(forestsFilePath, json)
        Ok ()
    with excp ->
        Error(excp.Message)

let private addForest
    (forestsFilePath: string)
    (forestDto: ForestDrivenInputDto) =
        result {
            let! forestOption = provideForests forestsFilePath
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
        
let private findForest
    (forestsFilePath: string)
    (searchTextDto: string) =
        result {
            let! forestOption = provideForests forestsFilePath 
            return
                match forestOption with
                | Some forests ->
                    List.filter (fun li ->
                        li.Hash.ToLower().Contains(searchTextDto.ToLower()) ||
                        li.Name.ToLower().Contains(searchTextDto.ToLower())) forests
                    |> Some
                | None ->
                    None
        }
        
let createLocalJsonFileForestAdapters defaultWorkingDirPath =
    {|
        provideForests = fun validConfigurationDto ->
            provideForests <| getForestFilePath defaultWorkingDirPath validConfigurationDto
        getForest = fun validConfigurationDto forestNameOrHashDto ->
            getForestByNameOrHash <| getForestFilePath defaultWorkingDirPath validConfigurationDto <| forestNameOrHashDto
        addForest = fun validConfigurationDto forestDto ->
           addForest <| getForestFilePath defaultWorkingDirPath validConfigurationDto <| forestDto           
        findForest = fun validConfigurationDto searchTextDto ->
           findForest <| getForestFilePath defaultWorkingDirPath validConfigurationDto <| searchTextDto
        // completeForest = fun validConfigurationDto forestDto ->
        //    completeForest <| getForestFilePath defaultWorkingDirPath validConfigurationDto <| forestDto
           
    |}