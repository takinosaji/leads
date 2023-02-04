module Leads.Core.Forests.Services

open Leads.Core.Config.Services
open Leads.Core.Utilities.Result
open Leads.Core.Utilities.ConstrainedTypes
open Leads.Core.Utilities.Dependencies

open Leads.Core.Config
open Leads.Core.Config.ConfigKey

open Leads.Core.Forests.ForestsDTO
open Leads.Core.Forests.ForestStatus.DTO

type ForestsProvider = string -> Result<ForestsDrivenDto, ErrorText>

type ListForestsEnvironment = {
    defaultWorkingDirPath: string
    provideConfig: ConfigurationProvider
    provideForests: ForestsProvider
}

let private toGetConfigEnvironment (forestEnvironment:ListForestsEnvironment) = {
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

type internal ListForests = ForestStatusDto -> Reader<ListForestsEnvironment, Result<Forests, ErrorText>>
let internal listForests: ListForests =
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
                        |> List.map Forest.fromDrivenDto
                        |> List.filter (filterByStatusPredicate statusDto)
                        |> Some
            | None -> return None            
        }    
    }
    