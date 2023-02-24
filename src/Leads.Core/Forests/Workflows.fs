module Leads.Core.Forests.Workflows

open Leads.Core.Config
open Leads.Core.Config.Services
open Leads.Core.Forests.ForestDTO
open Leads.Core.Forests.ForestsDto
open Leads.SecondaryPorts.Config
open Leads.SecondaryPorts.Forest
open Leads.Utilities.ConstrainedTypes
open Leads.Utilities.Dependencies
open Leads.Utilities.Result

open Leads.Core.Forests
open Leads.Core.Forests.Services
open Leads.SecondaryPorts.Forest.DTO

type AddForestEnvironment = {
    provideConfig: ConfigurationProvider
    addForest: ForestAppender
}

// type CompleteForestEnvironment = {
//     provideConfig: ConfigurationProvider
//     completeForest: ForestCompleter
// }


let private addToGetConfigEnvironment (forestEnvironment:AddForestEnvironment): GetConfigEnvironment = {
    provideConfig = forestEnvironment.provideConfig
}

// TODO: Write unit tests
type ListForestsWorkflow = ForestStatuses -> Reader<FindForestEnvironment, Result<ForestsPrimaryDto, string>>
let listForestsWorkflow: ListForestsWorkflow =
    fun statuses -> reader {
        let findCriteria = {
            text = All
            statuses = statuses |> ForestStatuses.toStatusesSecondaryDto
        }
        let! listForestResult = findForests findCriteria
        return listForestResult
               |> Result.map Forests.toPrimaryDtoList                 
               |> Result.mapError errorTextToString     
    }
               
type DescribeForestsWorkflow = string -> ForestStatuses -> Reader<FindForestEnvironment, Result<ForestPrimaryOutputDto list option, string>>
let describeForestsWorkflow: DescribeForestsWorkflow =
    fun searchText targetStatuses -> reader {                   
        let findCriteria = {
            text = ContainsText searchText
            statuses = targetStatuses |> ForestStatuses.toStatusesSecondaryDto
        }
        let! listForestResult = findForests findCriteria
        return listForestResult
               |> Result.map Forests.toPrimaryDtoList                 
               |> Result.mapError errorTextToString   
    }
            
    
// TODO: Write unit tests
type AddForestWorkflow = string -> Reader<AddForestEnvironment, Result<ValidForestOutputDto, string>>
let addForestWorkflow: AddForestWorkflow =
    fun name -> reader {
        let! environment = Reader.ask
        let! getConfigResult = getConfig()
                                |> Reader.withEnv addToGetConfigEnvironment
                                    
        return result {   
            let! name = ForestName.create name
            let! config = getConfigResult
            let configDto = Configuration.toValidSecondaryInputDto config
                            
            return!
                match Forest.create name with
                | Ok forest ->
                    forest
                    |> Forest.toSecondaryInputDto
                    |> environment.addForest configDto
                    |> Result.mapError stringToErrorText
                | Error e -> Error e
        } |> Result.mapError errorTextToString                     
    } 
            