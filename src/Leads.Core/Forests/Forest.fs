namespace Leads.Core.Forests

open System

open Leads.Utilities.ConstrainedTypes
open Leads.Utilities.Result

open Leads.Core.Models   
open Leads.SecondaryPorts.Forest.DTO

type ValidForest = {
    Hash: Hash
    Name: ForestName
    Status: ForestStatus
    Created: DateTime
    LastModified: DateTime
}
    
type InvalidForestModel = {
    Forest: ForestSODto
    Error: ErrorText
}

type ValidatedForest = 
    | ValidForestCase of ValidForest
    | InvalidForestCase of InvalidForestModel
    
type Forest = private Forest of ValidatedForest    

module Forest =
    module DTO =
        type ValidForestPODto = ForestSODto
        type InvalidForestOutputDto = { Forest: ForestSODto; Error: string }
            
        type ForestPODto =
            | ValidForestPODtoCase of ValidForestPODto
            | InvalidForestPODtoCase of InvalidForestOutputDto
        module ForestPODto =
            let fromSecondaryOutputDto
                (secondaryOutputDto: ForestSODto)
                :ForestPODto =
                    ValidForestPODtoCase secondaryOutputDto    
    open DTO

    let value (Forest forest) = forest
    
    let internal newForest (forestName: ForestName) = result {
        let hash = Hash.newRandom()
        let timeStamp = DateTime.UtcNow
        let status = ForestStatus.createActive()
        
        return {
            Hash = hash
            Name = forestName
            Status = status
            Created = timeStamp
            LastModified = timeStamp
        }
    }

    let internal fromSecondaryOutputDto (inboundDto: ForestSODto): Forest =
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
            Forest(ValidForestCase validatedFields)
        | Error errorText ->
            Forest(InvalidForestCase {
                Forest = inboundDto
                Error = errorText
            })
            
    let internal toSIDto (validForest:ValidForest) :ForestSIDto =
        {
           Hash = Hash.value validForest.Hash
           Name = ForestName.value validForest.Name
           Created = validForest.Created
           LastModified = validForest.LastModified
           Status = ForestStatus.value validForest.Status
        }                
    
    let internal toValidForestOutputDto validForest: ValidForestPODto =
        {
            Hash = Hash.value validForest.Hash
            Name = ForestName.value validForest.Name
            Created = validForest.Created
            LastModified = validForest.LastModified
            Status = ForestStatus.value validForest.Status
        }
    
    let internal toPrimaryOutputDto (forest:Forest) :ForestPODto =
        let forestValue = value forest
        match forestValue with
        | ValidForestCase validForest ->
            toValidForestOutputDto validForest
            |> ValidForestPODtoCase
        | InvalidForestCase invalidForest ->
            InvalidForestPODtoCase {
                Forest = invalidForest.Forest
                Error = errorTextToString invalidForest.Error
            }           