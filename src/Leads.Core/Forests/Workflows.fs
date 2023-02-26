module Leads.Core.Forests.Workflows

open System
open Leads.Core.Config
open Leads.Core.Config.Services
open Leads.Core.Forests.Forest.DTO
open Leads.Core.Forests.Forests.DTO
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
    provideAllowedConfigKeys: AllowedConfigKeysProvider
    provideConfig: ConfigurationProvider
    findForests: ForestsFinder
    addForest: ForestAppender
}
let private addToGetConfigEnvironment (forestEnvironment:AddForestEnvironment): GetConfigEnvironment = {
    provideAllowedConfigKeys = forestEnvironment.provideAllowedConfigKeys
    provideConfig = forestEnvironment.provideConfig
}

type UpdateForestEnvironment = {
    provideAllowedConfigKeys: AllowedConfigKeysProvider
    provideConfig: ConfigurationProvider
    findForests: ForestsFinder
    updateForest: ForestUpdater
}
let private updateToGetConfigEnvironment (forestEnvironment:UpdateForestEnvironment): GetConfigEnvironment = {
    provideAllowedConfigKeys = forestEnvironment.provideAllowedConfigKeys
    provideConfig = forestEnvironment.provideConfig
}

let updateToFindForestsEnvironment (forestEnvironment: UpdateForestEnvironment): FindForestEnvironment = {
    provideAllowedConfigKeys = forestEnvironment.provideAllowedConfigKeys
    provideConfig = forestEnvironment.provideConfig
    findForests = forestEnvironment.findForests
}


let addToFindForestsEnvironment (forestEnvironment: AddForestEnvironment): FindForestEnvironment = {
    provideAllowedConfigKeys = forestEnvironment.provideAllowedConfigKeys
    provideConfig = forestEnvironment.provideConfig
    findForests = forestEnvironment.findForests
}

// TODO: Write unit tests
type ListForestsWorkflow = ForestStatuses -> Reader<FindForestEnvironment, Result<ForestsOptionPODto, string>>
let listForestsWorkflow: ListForestsWorkflow =
    fun statuses -> reader {
        let! listForestResult =
            [{
                Name = Any
                Hash = Any
                Statuses = statuses |> ForestStatuses.toStatusesSecondaryDto
            }] |> findForests
            
        return listForestResult
               |> Result.map Forests.toOptionPODtoList                 
               |> Result.mapError errorTextToString     
    }
       
// TODO: Write unit tests        
type DescribeForestsWorkflow = string -> ForestStatuses -> Reader<FindForestEnvironment, Result<ForestsOptionPODto, string>>
let describeForestsWorkflow: DescribeForestsWorkflow =
    fun searchText targetStatuses -> reader {                   
        let! listForestResult =
            [{
                Name = Contains searchText
                Hash = Contains searchText
                Statuses = targetStatuses |> ForestStatuses.toStatusesSecondaryDto
            }] |> findForests
            
        return listForestResult  
               |> Result.map Forests.toOptionPODtoList                 
               |> Result.mapError errorTextToString   
    }
                
// TODO: Write unit tests
type AddForestWorkflow = string -> Reader<AddForestEnvironment, Result<ForestPODto, string>>
let addForestWorkflow: AddForestWorkflow =
    fun unvalidatedName -> reader {
        let! environment = Reader.ask
                                
        return result {   
            let! validatedName = ForestName.create unvalidatedName
            let! newForest = Forest.newForest validatedName        
            let forestValue = newForest |> Forest.value
            let forestHash = Hash.value forestValue.Hash

            let getConfigResult = getConfig()
                                    |> Reader.withEnv addToGetConfigEnvironment
                                    |> Reader.run environment   
            let! config = getConfigResult
            let configDto = Configuration.toValidSODto config                    
                            
            let findForestsResult =
                [{
                    Name = Any
                    Hash = Exact forestHash
                    Statuses = ForestStatuses.All |> ForestStatuses.toStatusesSecondaryDto };
                {
                    Name = Exact unvalidatedName
                    Hash = Any
                    Statuses = ForestStatuses.All |> ForestStatuses.toStatusesSecondaryDto }]
                |> findForests
                |> Reader.withEnv addToFindForestsEnvironment
                |> Reader.run environment                        
            let! foundForests = findForestsResult
            
            return!
                match foundForests with
                | None ->                                    
                    newForest
                    |> Forest.toSIDto
                    |> environment.addForest configDto
                    |> Result.mapError stringToErrorText                  
                | Some forests ->
                    Error (ErrorText $"The forests with Name {unvalidatedName} or hash {forestHash} already exist")
        } |> Result.mapError errorTextToString                     
    } 

type CompleteForestWorkflow = string -> Reader<UpdateForestEnvironment, Result<ForestPODto, string>>
let completeForestWorkflow: CompleteForestWorkflow =
    fun forestHash -> reader {
        let! environment = Reader.ask
        let! getConfigResult = getConfig()
                                |> Reader.withEnv updateToGetConfigEnvironment
                                      
        return result {
            let! config = getConfigResult
            let configDto = Configuration.toValidSODto config
            
            let findForestResult =
                [{
                    Name = Any
                    Hash = Exact forestHash
                    Statuses = ForestStatuses.Active |> ForestStatuses.toStatusesSecondaryDto }]
                |> findForests 
                |> Reader.withEnv updateToFindForestsEnvironment
                |> Reader.run environment
            
            let! foundForests = findForestResult
            return!
                match foundForests with
                | Some [forest] ->
                    let completedForest =
                        { Forest.value forest with
                            LastModified = DateTime.UtcNow
                            Status = ForestStatus.createCompleted() } |> Forest
                        
                    completedForest
                    |> Forest.toSIDto
                    |> environment.updateForest configDto
                    |> Result.map (fun _ -> completedForest |> Forest.toPODto)
                    |> Result.mapError stringToErrorText
                | Some forests ->
                    Error (ErrorText $"Found multiple forests with Hash {forestHash}. Hash is supposed to be unique. Forest names are {Forests.extractNamesString forests}.")
                | None ->
                    Error (ErrorText $"Active forest with Hash {forestHash} has not been found")
        } |> Result.mapError errorTextToString    
    }