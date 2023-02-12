namespace Leads.Core.Forests

open System

open Leads.Utilities.ConstrainedTypes
open Leads.Utilities.Result

open Leads.Core.Models   
open Leads.DrivenPorts.Forest.DTO

module ForestDto =
    type ValidForestDto = ForestDrivenDto
    type InvalidForestDto = { Forest: ForestDrivenDto; Error: string }
        
    type ForestDrivingDto =
        | ValidForestDto of ValidForestDto
        | InvalidForestDto of InvalidForestDto
open ForestDto

type ValidForest = {
    Hash: Hash
    Name: ForestName
    Status: ForestStatus
    Created: DateTime
    LastModified: DateTime
}
    
type InvalidForest = {
    Forest: ForestDrivenDto
    Error: ErrorText
}

type ValidatedForest = 
    | ValidForest of ValidForest
    | InvalidForest of InvalidForest
    
type Forest = private Forest of ValidatedForest    

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

    let fromDrivenDto (inboundDto:ForestDrivenDto): Forest =
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
            
    let toDrivenDto (validForest:ValidForest) :ForestDrivenDto =
        {
           Hash = Hash.value validForest.Hash
           Name = ForestName.value validForest.Name
           Created = validForest.Created
           LastModified = validForest.LastModified
           Status = ForestStatus.value validForest.Status
        }                
        
    let toDrivingDto (forest:Forest) :ForestDrivingDto =
        let forestValue = value forest
        match forestValue with
        | ValidForest validForest ->
            toDrivenDto validForest |> ValidForestDto
        | InvalidForest invalidForest ->
            InvalidForestDto {
                Forest = invalidForest.Forest
                Error = errorTextToString invalidForest.Error
            }           