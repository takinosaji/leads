module Leads.Core.Forests.Workflows

open Leads.Core.Config.Services
open Leads.Core.Utilities.Result
open Leads.Core.Utilities.ConstrainedTypes
open Leads.Core.Utilities.Dependencies

open Leads.Core.Config
open Leads.Core.Config.ConfigKey

open Leads.Core.Forests.DTO
open Leads.Core.Forests.ForestStatus.DTO

type ForestsProvider = string -> Result<ForestsDrivenDto, ErrorText>
type ForestAppender = Forest -> Result<unit, ErrorText>

type GetForestsEnvironment = {
    defaultWorkingDirPath: string
    provideConfig: ConfigurationProvider
    provideForests: ForestsProvider
}

type AddForestEnvironment = {
    defaultWorkingDirPath: string
    provideConfig: ConfigurationProvider
    provideForests: ForestsProvider
    addForest: ForestAppender
}

let private toGetConfigEnvironment (forestEnvironment:GetForestsEnvironment) = {
    provideConfig =  forestEnvironment.provideConfig
}

let private filterByStatusPredicate forestStatusDto (forest: Forest) =
    let validatedForest = Forest.value forest
    match validatedForest with
    | ValidForest validForest ->
        match forestStatusDto with
        | ForestStatusDto.All -> true
        | _ ->
            ((forestStatusDto &&& ForestStatusDto.Active = ForestStatusDto.Active) && validForest.Status = ForestStatus.Active) || 
            ((forestStatusDto &&& ForestStatusDto.Completed = ForestStatusDto.Completed) && validForest.Status = ForestStatus.Completed) || 
            ((forestStatusDto &&& ForestStatusDto.Archived = ForestStatusDto.Archived) && validForest.Status = ForestStatus.Archived)                      
    | InvalidForest _ -> false 

// TODO: Write unit tests
type ListForestsWorkflow = ForestStatusDto -> Reader<GetForestsEnvironment, Result<ForestsDrivingDto, string>>
let listForestsWorkflow: ListForestsWorkflow =
    fun statusDto -> reader {
        let! environment = Reader.ask
        let! getWorkingDirPathResult = getConfigValue WorkingDirKey
                                    |> Reader.withEnv toGetConfigEnvironment
        return result {
            let! workingDirPathOption = getWorkingDirPathResult
            let workingDirPath = ConfigValue.valueOrDefaultOption workingDirPathOption environment.defaultWorkingDirPath
            
            let! unvalidatedForests = ConfigValue.value workingDirPath |> environment.provideForests
            match unvalidatedForests with
            | Some forests ->                
                return forests
                        |> List.map Forest.create
                        |> List.filter (filterByStatusPredicate statusDto)                        
                        |> List.map Forest.toOutputDto
                        |> Some
            | None -> return None            
        } |> Result.mapError errorTextToString     
    }
    
// TODO: Write unit tests
type AddForestWorkflow = ForestDrivingDto -> Reader<AddForestEnvironment, Result<unit, string>>