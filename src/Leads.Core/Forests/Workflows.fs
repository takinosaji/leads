module Leads.Core.Forests.Workflows

open Leads.Core.Config
open Leads.Core.Config.Services
open Leads.Core.Forests.Forest.DTO
open Leads.Core.Forests.ForestsOption.DTO
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

type UpdateForestEnvironment = {
    provideConfig: ConfigurationProvider
    findForests: ForestsFinder
    updateForest: ForestUpdater
}

let private addToGetConfigEnvironment (forestEnvironment:AddForestEnvironment): GetConfigEnvironment = {
    provideConfig = forestEnvironment.provideConfig
}

let toFindForestsEnvironment (forestEnvironment: UpdateForestEnvironment): FindForestEnvironment = {
    provideConfig = forestEnvironment.provideConfig
    findForests = forestEnvironment.findForests
}

// TODO: Write unit tests
type ListForestsWorkflow = ForestStatuses -> Reader<FindForestEnvironment, Result<ForestsOptionPODto, string>>
let listForestsWorkflow: ListForestsWorkflow =
    fun statuses -> reader {
        let findCriteria = {
            text = All
            statuses = statuses |> ForestStatuses.toStatusesSecondaryDto
        }
        let! listForestResult = findForests findCriteria
        return listForestResult
               |> Result.map ForestsOption.toPrimaryOutputDtoList                 
               |> Result.mapError errorTextToString     
    }
       
// TODO: Write unit tests        
type DescribeForestsWorkflow = string -> ForestStatuses -> Reader<FindForestEnvironment, Result<ValidForestPODto list option, string>>
let describeForestsWorkflow: DescribeForestsWorkflow =
    fun searchText targetStatuses -> reader {                   
        let findCriteria = {
            text = ContainsText searchText
            statuses = targetStatuses |> ForestStatuses.toStatusesSecondaryDto
        }
        
        let! listForestResult = findForests findCriteria
        return listForestResult
               |> Result.map ForestsOption.filterOutInvalid  
               |> Result.map ForestsOption.toValidForestPODtoList                 
               |> Result.mapError errorTextToString   
    }
                
// TODO: Write unit tests
type AddForestWorkflow = string -> Reader<AddForestEnvironment, Result<ValidForestPODto, string>>
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
                match Forest.newForest name with
                | Ok forest ->
                    forest
                    |> Forest.toSecondaryInputDto
                    |> environment.addForest configDto
                    |> Result.mapError stringToErrorText
                | Error e -> Error e
        } |> Result.mapError errorTextToString                     
    } 

type CompleteForestWorkflow = string -> Reader<UpdateForestEnvironment, Result<ValidForestPODto, string>>
let completeForestWorkflow: CompleteForestWorkflow =