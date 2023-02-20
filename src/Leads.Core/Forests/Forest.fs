namespace Leads.Core.Forests

open System

open Leads.Utilities.ConstrainedTypes
open Leads.Utilities.Result

open Leads.Core.Models   
open Leads.DrivenPorts.Forest.DTO

module ForestDto =
    type ValidForestOutputDto = ForestDrivenOutputDto
    type InvalidForestOutputDto = { Forest: ForestDrivenOutputDto; Error: string }
        
    type ForestDrivingOutputDto =
        | ValidForest of ValidForestOutputDto
        | InvalidForest of InvalidForestOutputDto
    module ForestDrivingOutputDto =
        let fromDrivenOutputDto
            (drivenOutputDto: ForestDrivenOutputDto)
            :ValidForestOutputDto =
                drivenOutputDto
    
open ForestDto

type ValidForestModel = {
    Hash: Hash
    Name: ForestName
    Status: ForestStatus
    Created: DateTime
    LastModified: DateTime
}
    
type InvalidForestModel = {
    Forest: ForestDrivenOutputDto
    Error: ErrorText
}

type ValidatedForestModel = 
    | ValidForest of ValidForestModel
    | InvalidForest of InvalidForestModel
    
type Forest = private Forest of ValidatedForestModel    

module Forest =
    let value (Forest forest) = forest
    
    let create (forestName: ForestName) = result {
        let timeStamp = DateTime.UtcNow
        let! hash = Hash.newRandom()
        let! status = ForestStatus.createActive()
        
        return {
            Hash = hash
            Name = forestName
            Status = status
            Created = timeStamp
            LastModified = timeStamp
        }
    }

    let fromDrivenDto (inboundDto:ForestDrivenOutputDto): Forest =
        let fieldsValidationResult = result {
            let! forestStatus = ForestStatus.create inboundDto.Status 
            let! forestHash = Hash.create inboundDto.Hash
            let! forestName = ForestName.create inboundDto.Name          
           
            return {
                Hash = forestHash
                Name = forestName
                Status = forestStatus
                Created = inboundDto.Created
                LastModified = inboundDto.LastModified 
            }
        }
        
        match fieldsValidationResult with
        | Ok validatedFields ->
            Forest(ValidForest validatedFields)
        | Error errorText ->
            Forest(InvalidForest {
                Forest = inboundDto
                Error = errorText
            })
            
    let toDrivenInputDto (validForest:ValidForestModel) :ForestDrivenInputDto =
        {
           Hash = Hash.value validForest.Hash
           Name = ForestName.value validForest.Name
           Created = validForest.Created
           LastModified = validForest.LastModified
           Status = ForestStatus.value validForest.Status
        }                
        
    let toDrivingOutputDto (forest:Forest) :ForestDrivingOutputDto =
        let forestValue = value forest
        match forestValue with
        | ValidForest validForest ->
            toDrivenInputDto validForest |> ValidForest
        | InvalidForest invalidForest ->
            InvalidForest {
                Forest = invalidForest.Forest
                Error = errorTextToString invalidForest.Error
            }           