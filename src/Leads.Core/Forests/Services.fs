module Leads.Core.Forests.Services

open Leads.Core.Config.Services
open Leads.DrivenPorts.Config
open Leads.DrivenPorts.Forest
open Leads.DrivenPorts.Forest.DTO
open Leads.Utilities.Result
open Leads.Utilities.ConstrainedTypes
open Leads.Utilities.Dependencies

open Leads.Core.Config

type ListForestsEnvironment = {
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
        let! getConfigResult = getConfig()
                                    |> Reader.withEnv toGetConfigEnvironment
        return result {
            let! config = getConfigResult
            let! unvalidatedForests =
                config
                |> Configuration.toValidDrivenInputDto
                |> environment.provideForests
                |> Result.mapError stringToErrorText
            match unvalidatedForests with
            | Some forests ->                
                return forests
                        |> List.map Forest.fromDrivenDto
                        |> List.filter (filterByStatusPredicate statusDto)
                        |> Some
            | None -> return None            
        }    
    }
    