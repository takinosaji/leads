module Leads.Core.Forests.Workflows

open Leads.Core.Forests.ForestDto
open Leads.Core.Forests.ForestsDto
open Leads.DrivenPorts.Config
open Leads.DrivenPorts.Forest
open Leads.Utilities.ConstrainedTypes
open Leads.Utilities.Dependencies
open Leads.Utilities.Result

open Leads.Core.Forests
open Leads.Core.Forests.Services
open Leads.DrivenPorts.Forest.DTO

type AddForestEnvironment = {
    provideConfig: ConfigurationProvider
    provideForests: ForestsProvider
    addForest: ForestAppender
}

let private toListForestsEnvironment (addForestEnvironment:AddForestEnvironment) = {
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
            
            return!
                match Forests.exists name forests with
                | false ->
                    match Forest.create name with
                    | Ok forest ->
                        environment.addForest (Forest.toDrivenDto forest)
                        |> Result.mapError stringToErrorText
                    | Error e -> Error e
                | true ->
                    Error (ErrorText $"Forest with the name {nameDto} already exists")
           
        } |> Result.mapError errorTextToString                     
    } 
            