namespace Leads.Core.Forests

open System

open Leads.Utilities.ConstrainedTypes
open Leads.Utilities.Result

open Leads.Core.Models   
open Leads.SecondaryPorts.Forest.DTO

module ForestDTO =
    type ValidForestOutputDto = ForestSecondaryOutputDto
    type InvalidForestOutputDto = { Forest: ForestSecondaryOutputDto; Error: string }
        
    type ForestPrimaryOutputDto =
        | ValidForestOutputDto of ValidForestOutputDto
        | InvalidForestOutputDto of InvalidForestOutputDto
    module ForestPrimaryOutputDto =
        let fromSecondaryOutputDto
            (secondaryOutputDto: ForestSecondaryOutputDto)
            :ForestPrimaryOutputDto =
                ValidForestOutputDto secondaryOutputDto    
open ForestDTO

type ValidForestModel = {
    Hash: Hash
    Name: ForestName
    Status: ForestStatus
    Created: DateTime
    LastModified: DateTime
}
    
type InvalidForestModel = {
    Forest: ForestSecondaryOutputDto
    Error: ErrorText
}

type ValidatedForestModel = 
    | ValidForest of ValidForestModel
    | InvalidForest of InvalidForestModel
    
type Forest = private Forest of ValidatedForestModel    

module Forest =
    let value (Forest forest) = forest
    
    let create (forestName: ForestName) = result {
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

    let fromSecondaryOutputDto (inboundDto: ForestSecondaryOutputDto): Forest =
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
            
    let toSecondaryInputDto (validForest:ValidForestModel) :ForestSecondaryInputDto =
        {
           Hash = Hash.value validForest.Hash
           Name = ForestName.value validForest.Name
           Created = validForest.Created
           LastModified = validForest.LastModified
           Status = ForestStatus.value validForest.Status
        }                
        
    let toPrimaryOutputDto (forest:Forest) :ForestPrimaryOutputDto =
        let forestValue = value forest
        match forestValue with
        | ValidForest validForest ->
            ValidForestOutputDto {
                Hash = Hash.value validForest.Hash
                Name = ForestName.value validForest.Name
                Created = validForest.Created
                LastModified = validForest.LastModified
                Status = ForestStatus.value validForest.Status
            }
        | InvalidForest invalidForest ->
            InvalidForestOutputDto {
                Forest = invalidForest.Forest
                Error = errorTextToString invalidForest.Error
            }           