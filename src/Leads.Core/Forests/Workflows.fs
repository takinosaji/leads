module Leads.Core.Forests.Workflows

open System
open Leads.Core.Utilities.ConstrainedTypes
open Leads.Core.Utilities.Dependencies
open Leads.Core.Utilities.Result

open Leads.Core.Config.Services

open Leads.Core.Forests
open Leads.Core.Forests.Services
open Leads.Core.Forests.ForestDTO
open Leads.Core.Forests.ForestsDTO
open Leads.Core.Forests.ForestStatus.DTO

type ForestAppender = Forest -> Result<unit, ErrorText>

type AddForestEnvironment = {
    defaultWorkingDirPath: string
    provideConfig: ConfigurationProvider
    provideForests: ForestsProvider
    addForest: ForestAppender
}

let private toListForestsEnvironment (addForestEnvironment:AddForestEnvironment) = {
    defaultWorkingDirPath = addForestEnvironment.defaultWorkingDirPath
    provideConfig =  addForestEnvironment.provideConfig
    provideForests = addForestEnvironment.provideForests
}

// TODO: Write unit tests
type ListForestsWorkflow = ForestStatusDto -> Reader<ListForestsEnvironment, Result<ForestsDrivingDto, string>>
let listForestsWorkflow: ListForestsWorkflow =
    fun statusDto -> reader {
        let! listForestResult = listForests(statusDto)
        return listForestResult
               |> Result.map Forests.toDrivingDtoList                 
               |> Result.mapError errorTextToString     
    }
        
    
// TODO: Write unit tests
type AddForestWorkflow = string -> Reader<AddForestEnvironment, Result<ValidForestDto, string>>
let addForestWorkflow: AddForestWorkflow =
    fun nameDto -> reader {
        let! environment = Reader.ask
        let! listForestResult = listForests ForestStatusDto.All
                                    |> Reader.withEnv toListForestsEnvironment
        
        return result {
            let! forests = listForestResult
            let! name = ForestName.create nameDto
            let! addedForest = match Forests.exists name forests with
            | false ->
                Forest.create 
                environment.addForest 
                Ok { Hash = "1111"; Name = "1111"; Created = DateTime.Now; LastModified = DateTime.Now; Status = "1111"; }
            | true ->
                Error (ErrorText $"Forest with the name {nameDto} already exists")
            
            return addedForest
        } |> Result.mapError errorTextToString                     
    } 
            