namespace Leads.Core.Forests

open System

open Leads.Core.Utilities.ConstrainedTypes
open Leads.Core.Utilities.Result

open Leads.Core.Models

module DTO =
    type ForestInboundDto = { Hash: string; Name: string; Created: string; LastModified: string; Status: string }
    type ForestsInboundDto = ForestInboundDto list option
        
    type ForestOutboundDto =
        | ValidForestDto of {| Hash: string; Name: string; Created: DateTime; LastModified: DateTime; Status: string |}
        | InvalidForestDto of {| Forest: ForestInboundDto; Error: string |}
    
    type ForestsOutboundDto = ForestOutboundDto list option
    
open DTO

type ValidForest = {
    Hash: Hash
    Name: ForestName
    Status: ForestStatus
    Created: DateTime
    LastModified: DateTime
}
    
type InvalidForest = {
    Forest: ForestInboundDto
    Error: ErrorText
}

type ValidatedForest = 
    | ValidForest of ValidForest
    | InvalidForest of InvalidForest
    
type Forest = private Forest of ValidatedForest    
type Forests = Forest list option
       
module Forest =
    let value (Forest forest) = forest
    
    let create (inboundDto:ForestInboundDto): Forest =
        let fieldsValidationResult = result {
            let! forestStatus = ForestStatus.create inboundDto.Status 
            let! forestHash = Hash.create inboundDto.Hash
            let! forestName = ForestName.create inboundDto.Name
            let! created = createDateTime inboundDto.Created            
            let! lastModified = createDateTime inboundDto.LastModified            
           
            return {
                Hash = forestHash
                Name = forestName
                Status = forestStatus
                Created = created
                LastModified = lastModified
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
                
    let toOutputDto (forest:Forest) :ForestOutboundDto =
        let forestValue = value forest
        match forestValue with
        | ValidForest validForest ->
            ValidForestDto {|
                   Hash = Hash.value validForest.Hash
                   Name = ForestName.value validForest.Name
                   Created = validForest.Created
                   LastModified = validForest.LastModified
                   Status = ForestStatus.toDto validForest.Status
                |}
        | InvalidForest invalidForest ->
            InvalidForestDto {|
                Forest = invalidForest.Forest
                Error = errorTextToString invalidForest.Error
            |}
    