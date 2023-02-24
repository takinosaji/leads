module Leads.Core.Forests.Services

open Leads.Core.Config.Services
open Leads.Core.Forests.ForestDTO
open Leads.SecondaryPorts.Config
open Leads.SecondaryPorts.Forest
open Leads.SecondaryPorts.Forest.DTO
open Leads.Utilities.Result
open Leads.Utilities.ConstrainedTypes
open Leads.Utilities.Dependencies

open Leads.Core.Config

type FindForestEnvironment = {
    provideConfig: ConfigurationProvider
    findForests: ForestsFinder
}

let private toGetConfigEnvironment (forestEnvironment:FindForestEnvironment) = {
    provideConfig =  forestEnvironment.provideConfig
}


// type ListForestsEnvironment = {
//     provideConfig: ConfigurationProvider
//     listForests: ForestsProvider
// }
// let private filterByStatusPredicate forestStatusDto (forest: Forest) =
//     let validatedForest = Forest.value forest
//     match validatedForest with
//     | ValidForest validForest ->
//         match forestStatusDto with
//         | ForestStatusDto.All -> true
//         | _ ->
//             ((forestStatusDto &&& ForestStatusDto.Active = ForestStatusDto.Active) && validForest.Status = ForestStatus.Active) || 
//             ((forestStatusDto &&& ForestStatusDto.Completed = ForestStatusDto.Completed) && validForest.Status = ForestStatus.Completed) || 
//             ((forestStatusDto &&& ForestStatusDto.Archived = ForestStatusDto.Archived) && validForest.Status = ForestStatus.Archived)                      
//     | InvalidForest _ -> false 
//
// type internal ListForests = ForestStatusDto -> Reader<ListForestsEnvironment, Result<Forests, ErrorText>>
// let internal listForests: ListForests =
//     fun statusDto -> reader {
//         let! environment = Reader.ask
//         let! getConfigResult = getConfig()
//                                     |> Reader.withEnv toGetConfigEnvironment
//         return result {
//             let! config = getConfigResult
//             let! unvalidatedForests =
//                 config
//                 |> Configuration.toValidDrivenInputDto
//                 |> environment.provideForests
//                 |> Result.mapError stringToErrorText
//             match unvalidatedForests with
//             | Some forests ->                
//                 return forests
//                         |> List.map Forest.fromDrivenDto
//                         |> List.filter (filterByStatusPredicate statusDto)
//                         |> Some
//             | None -> return None            
//         }    
//     }

type internal FindForests = FindCriteriaDto -> Reader<FindForestEnvironment, Result<Forests, ErrorText>>
let internal findForests: FindForests =
    fun findCriteria -> reader {
        let! environment = Reader.ask
        let! getConfigResult = getConfig()
                                    |> Reader.withEnv toGetConfigEnvironment
        return result {
            let! config = getConfigResult
            let configDto = config |> Configuration.toValidSecondaryInputDto
            
            let! unvalidatedForests =
                findCriteria
                |> environment.findForests configDto 
                |> Result.mapError stringToErrorText
                
            match unvalidatedForests with
            | Some forests ->                
                return forests
                        |> List.map Forest.fromSecondaryOutputDto
                        |> Some
            | None -> return None            
        }    
    }
    
    
    
        
    
// type DescribeForestWorkflow = string -> Reader<DescribeForestEnvironment, Result<ForestPrimaryOutputDto list option, string>>
// let describeForestWorkflow: DescribeForestWorkflow =
//     fun searchText -> reader {
//         let! environment = Reader.ask
//         let! getConfigResult = getConfig()
//                                 |> Reader.withEnv describeToGetConfigEnvironment
//                                     
//         return result {
//             let! config = getConfigResult
//             let configDto = Configuration.toValidSecondaryInputDto config
//             
//             let! forestSearchResult = 
//                 searchText                
//                 |> environment.findForest configDto
//                 |> Result.mapError stringToErrorText
//             
//             return!
//                 match forestSearchResult with
//                 | Some forests ->
//                     forests
//                     |> List.map Forest.fromSecondaryDto
//                     |> List.map Forest.toPrimaryOutputDto
//                     |> Some
//                     |> Ok
//                 | None ->
//                     None
//                     |> Ok                                    
//         } |> Result.mapError errorTextToString                     
//     } 