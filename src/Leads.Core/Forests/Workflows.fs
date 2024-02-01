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

type DeleteForestEnvironment = {
    provideAllowedConfigKeys: AllowedConfigKeysProvider
    provideConfig: ConfigurationProvider
    findForests: ForestsFinder
    deleteForest: ForestDeleter
}

let private updateToGetConfigEnvironment (forestEnvironment:UpdateForestEnvironment): GetConfigEnvironment = {
    provideAllowedConfigKeys = forestEnvironment.provideAllowedConfigKeys
    provideConfig = forestEnvironment.provideConfig
}

let private deleteToGetConfigEnvironment (forestEnvironment:DeleteForestEnvironment): GetConfigEnvironment = {
    provideAllowedConfigKeys = forestEnvironment.provideAllowedConfigKeys
    provideConfig = forestEnvironment.provideConfig
}
let updateToFindForestsEnvironment (forestEnvironment: UpdateForestEnvironment): FindForestEnvironment = {
    provideAllowedConfigKeys = forestEnvironment.provideAllowedConfigKeys
    provideConfig = forestEnvironment.provideConfig
    findForests = forestEnvironment.findForests
}

let deleteToFindForestsEnvironment (forestEnvironment: DeleteForestEnvironment): FindForestEnvironment = {
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
                Statuses = statuses |> ForestStatuses.toSIDto
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
                Name = Any
                Hash = Contains searchText
                Statuses = targetStatuses |> ForestStatuses.toSIDto };
            {
                Name = Contains searchText
                Hash = Any
                Statuses = targetStatuses |> ForestStatuses.toSIDto }]
            |> findForests
            
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
                    Statuses = ForestStatuses.All |> ForestStatuses.toSIDto };
                {
                    Name = Exact unvalidatedName
                    Hash = Any
                    Statuses = ForestStatuses.All |> ForestStatuses.toSIDto }]
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

type ChangeForestStatusWorkflow = string -> ForestStatus -> ForestStatus -> Reader<UpdateForestEnvironment, Result<ForestPODto, string>>
let changeForestStatusWorkflow: ChangeForestStatusWorkflow =
    fun forestHash forestStatusToFind newForestStatus -> reader {
        let! environment = Reader.ask
                                      
        return result {            
            let! config = getConfig()
                          |> Reader.run (environment |> updateToGetConfigEnvironment)                          
            let configDto = Configuration.toValidSODto config
            
            let findForestResult =
                [{
                    Name = Any
                    Hash = Exact forestHash
                    Statuses = [forestStatusToFind] }]
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
                            Status = newForestStatus } |> Forest
                        
                    completedForest
                    |> Forest.toSIDto
                    |> environment.updateForest configDto
                    |> Result.map (fun _ -> completedForest |> Forest.toPODto)
                    |> Result.mapError stringToErrorText
                | Some forests ->
                    Error (ErrorText $"Found multiple forests with Hash {forestHash}. Hash is supposed to be unique. Forest names are {Forests.extractNamesString forests}.")
                | None ->
                    Error (ErrorText $"Forest with Hash {forestHash} and Status {forestStatusToFind} has not been found")
        } |> Result.mapError errorTextToString    
    }
    
type DeleteForestWorkflow = string -> Reader<DeleteForestEnvironment, Result<ForestPODto, string>>
let deleteForestWorkflow: DeleteForestWorkflow =
    fun forestHash -> reader {
        let! environment = Reader.ask
                                      
        return result {            
            let! config = getConfig()
                          |> Reader.run (environment |> deleteToGetConfigEnvironment)                          
            let configDto = Configuration.toValidSODto config
            
            let findForestResult =
                [{
                    Name = Any
                    Hash = Exact forestHash
                    Statuses = [ForestStatus.Active; ForestStatus.Archived; ForestStatus.Completed] }]
                |> findForests 
                |> Reader.withEnv deleteToFindForestsEnvironment
                |> Reader.run environment
            
            let! foundForests = findForestResult
            return!
                match foundForests with
                | Some [forest] ->                        
                    forest
                    |> Forest.toSIDto
                    |> environment.deleteForest configDto
                    |> Result.map (fun _ -> forest |> Forest.toPODto)
                    |> Result.mapError stringToErrorText
                | Some forests ->
                    Error (ErrorText $"Found multiple forests with Hash {forestHash}. Hash is supposed to be unique. Forest names are {Forests.extractNamesString forests}.")
                | None ->
                    Error (ErrorText $"Forest with Hash {forestHash} has not been found")
        } |> Result.mapError errorTextToString    
    }