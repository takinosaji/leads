module Leads.Core.Forests.Workflows

open Leads.Core.Config
open Leads.Core.Config.Services
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
    addForest: ForestAppender
}

// type CompleteForestEnvironment = {
//     provideConfig: ConfigurationProvider
//     completeForest: ForestCompleter
// }

type DescribeForestEnvironment = {
    provideConfig: ConfigurationProvider
    findForest: ForestFinder
}

let private addToGetConfigEnvironment (forestEnvironment:AddForestEnvironment): GetConfigEnvironment = {
    provideConfig = forestEnvironment.provideConfig
}

let private describeToGetConfigEnvironment (forestEnvironment:DescribeForestEnvironment): GetConfigEnvironment = {
    provideConfig = forestEnvironment.provideConfig
}

// TODO: Write unit tests
type ListForestsWorkflow = ForestStatusDto -> Reader<ListForestsEnvironment, Result<ForestsDrivingDto, string>>
let listForestsWorkflow: ListForestsWorkflow =
    fun statusDto -> reader {
        let! listForestResult = listForests statusDto
        return listForestResult
               |> Result.map Forests.toDrivingDtoList                 
               |> Result.mapError errorTextToString     
    }
        
    
// TODO: Write unit tests
type AddForestWorkflow = string -> Reader<AddForestEnvironment, Result<ValidForestOutputDto, string>>
let addForestWorkflow: AddForestWorkflow =
    fun nameDto -> reader {
        let! environment = Reader.ask
        let! getConfigResult = getConfig()
                                |> Reader.withEnv addToGetConfigEnvironment
                                    
        return result {   
            let! name = ForestName.create nameDto
            let! config = getConfigResult
            let configDto = Configuration.toValidDrivenInputDto config
                            
            return!
                match Forest.create name with
                | Ok forest ->
                    forest
                    |> Forest.toDrivenInputDto
                    |> environment.addForest configDto
                    |> Result.map ForestDrivingOutputDto.fromDrivenOutputDto
                    |> Result.mapError stringToErrorText
                | Error e -> Error e
        } |> Result.mapError errorTextToString                     
    } 

type DescribeForestWorkflow = string -> Reader<DescribeForestEnvironment, Result<ForestDrivingOutputDto list option, string>>
let describeForestWorkflow: DescribeForestWorkflow =
    fun searchText -> reader {
        let! environment = Reader.ask
        let! getConfigResult = getConfig()
                                |> Reader.withEnv describeToGetConfigEnvironment
                                    
        return result {
            let! config = getConfigResult
            let configDto = Configuration.toValidDrivenInputDto config
            
            let! forestSearchResult = 
                searchText                
                |> environment.findForest configDto
                |> Result.mapError stringToErrorText
            
            return!
                match forestSearchResult with
                | Some forests ->
                    forests
                    |> List.map ForestDrivingOutputDto.fromDrivenOutputDto
                    |> Some
                    |> Ok
                | None ->
                    None
                    |> Ok                                    
        } |> Result.mapError errorTextToString                     
    } 