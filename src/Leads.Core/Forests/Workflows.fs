module Leads.Core.Forests.Workflows

open System
open Leads.Core.Config
open Leads.Core.Config.Services
open Leads.Core.Forests.Forest.DTO
open Leads.Core.Forests.ForestsOption.DTO
open Leads.Core.Models
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
let private addToGetConfigEnvironment (forestEnvironment:AddForestEnvironment): GetConfigEnvironment = {
    provideConfig = forestEnvironment.provideConfig
}

type UpdateForestEnvironment = {
    provideConfig: ConfigurationProvider
    findForests: ForestsFinder
    updateForest: ForestUpdater
}
let private updateToGetConfigEnvironment (forestEnvironment:UpdateForestEnvironment): GetConfigEnvironment = {
    provideConfig = forestEnvironment.provideConfig
}

let updateToFindForestsEnvironment (forestEnvironment: UpdateForestEnvironment): FindForestEnvironment = {
    provideConfig = forestEnvironment.provideConfig
    findForests = forestEnvironment.findForests
}

// TODO: Write unit tests
type ListForestsWorkflow = ForestStatuses -> Reader<FindForestEnvironment, Result<ForestsOptionPODto, string>>
let listForestsWorkflow: ListForestsWorkflow =
    fun statuses -> reader {
        let findCriteria = {
            Name = Any
            Hash = Any
            Statuses = statuses |> ForestStatuses.toStatusesSecondaryDto
        }
        let! listForestResult = findForests findCriteria
        return listForestResult
               |> Result.map ForestsOption.toPrimaryOutputDtoList                 
               |> Result.mapError errorTextToString     
    }
       
// TODO: Write unit tests        
type DescribeForestsWorkflow = string -> ForestStatuses -> Reader<FindForestEnvironment, Result<ValidForestsOptionPODto, string>>
let describeForestsWorkflow: DescribeForestsWorkflow =
    fun searchText targetStatuses -> reader {                   
        let findCriteria = {
            Name = Contains searchText
            Hash = Contains searchText
            Statuses = targetStatuses |> ForestStatuses.toStatusesSecondaryDto
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
            let configDto = Configuration.toValidSODto config
                            
            return!
                match Forest.newForest name with
                | Ok forest ->
                    forest
                    |> Forest.toSIDto
                    |> environment.addForest configDto
                    |> Result.mapError stringToErrorText
                | Error e -> Error e
        } |> Result.mapError errorTextToString                     
    } 

type CompleteForestWorkflow = string -> Reader<UpdateForestEnvironment, Result<ValidForestPODto, string>>
let completeForestWorkflow: CompleteForestWorkflow =
    fun forestHash -> reader {
        let! environment = Reader.ask
        let! getConfigResult = getConfig()
                                |> Reader.withEnv updateToGetConfigEnvironment
        
        let findCriteria = {
            Name = Any
            Hash = Exact forestHash
            Statuses = ForestStatuses.Active |> ForestStatuses.toStatusesSecondaryDto
        }        
        let! findForestResult = findForests findCriteria
                                |> Reader.withEnv updateToFindForestsEnvironment
                                
        return result {
            let! config = getConfigResult
            let configDto = Configuration.toValidSODto config
            
            let! foundForests = findForestResult
                                |> Result.map ForestsOption.filterOutInvalid
            return!
                match foundForests with
                | None ->
                    Error (ErrorText "Active forest with Hash={forestHash} has not been found")
                | Some [validForest] ->
                    let completedForest =
                        { validForest with
                            LastModified = DateTime.UtcNow
                            Status = ForestStatus.createCompleted() }
                        
                    completedForest
                    |> Forest.toSIDto
                    |> environment.updateForest configDto
                    |> Result.map (fun _ -> completedForest |> Forest.toPrimaryOutputDto)
                    |> Result.mapError stringToErrorText
                | Some forests ->
                    let names = String.Join(", ", forests |> List.map (fun f -> f.Name))
                    Error (ErrorText $"Found multiple forests with Hash={forestHash}. Hash is supposed to be unique. Forest names are {names}.")
        } |> Result.mapError errorTextToString    
    }