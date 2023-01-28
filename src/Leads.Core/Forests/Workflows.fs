module Leads.Core.Forests.Workflows

open Leads.Core.Config.Services
open Leads.Core.Utilities.Result
open Leads.Core.Utilities.ConstrainedTypes
open Leads.Core.Utilities.Dependencies

open Leads.Core.Config
open Leads.Core.Config.ConfigKey
open Leads.Core.Config.Workflows

open Leads.Core.Forests.DTO
open Leads.Core.Forests.ForestStatus.DTO

type ForestsProvider = string -> Result<ForestsInboundDto, ErrorText>

type ForestEnvironment = {
    configFilePath: string
    defaultWorkingDirPath: string
    provideConfig: ConfigurationProvider
    provideForests: ForestsProvider
}

let private toGetConfigEnvironment forestEnvironment = {
    configFilePath = forestEnvironment.configFilePath
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
type ListForestsWorkflow = ForestStatusDto -> Reader<ForestEnvironment, Result<ForestsOutboundDto, string>>
let listForestsWorkflow: ListForestsWorkflow =
    fun statusDto -> reader {
        let! environment = Reader.ask
        let! getWorkingDirPathResult = getConfigValue WorkingDirKey
                                    |> Reader.withEnv toGetConfigEnvironment
        return result {
            let! workingDirPathOption = getWorkingDirPathResult
            let workingDirPath = ConfigValue.valueOrDefaultOption workingDirPathOption environment.configFilePath
            
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
    