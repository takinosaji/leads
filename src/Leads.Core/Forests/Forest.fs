namespace Leads.Core.Forests

open System

open Leads.Utilities.ConstrainedTypes
open Leads.Utilities.Result

open Leads.Core.Models   
open Leads.SecondaryPorts.Forest.DTO

type ForestData = {
    Hash: Hash
    Name: ForestName
    Status: ForestStatus
    Created: DateTime
    LastModified: DateTime
}
    
type Forest = private Forest of ForestData    

module Forest =
    module DTO =
        type ForestPODto = ForestSODto
        type InvalidForestOutputDto = { Forest: ForestSODto; Error: string }
            
        module ForestPODto =
            let fromSecondaryOutputDto
                (secondaryOutputDto: ForestSODto)
                :ForestPODto =
                    secondaryOutputDto    
    open DTO

    let value (Forest forest) = forest
    
    let internal newForest (forestName: ForestName) = result {
        let! hash = Hash.newRandom()
        let timeStamp = DateTime.UtcNow
        let status = ForestStatus.createActive()
        
        return Forest {
            Hash = hash
            Name = forestName
            Status = status
            Created = timeStamp
            LastModified = timeStamp
        }
    }

    let internal fromSecondaryOutputDto (forestDto: ForestSODto) =
        result {
            let! forestStatus = ForestStatus.create forestDto.Status 
            let! forestHash = Hash.create forestDto.Hash
            let! forestName = ForestName.create forestDto.Name          
           
            return Forest {
                Hash = forestHash
                Name = forestName
                Status = forestStatus
                Created = forestDto.Created
                LastModified = forestDto.LastModified 
            }
        }
            
    let internal toSIDto (forest: Forest) :ForestSIDto =
        let forestData = value forest
        {
           Hash = Hash.value forestData.Hash
           Name = ForestName.value forestData.Name
           Created = forestData.Created
           LastModified = forestData.LastModified
           Status = ForestStatus.value forestData.Status
        }                
    
    let internal toPODto forest: ForestPODto =
        let forestData = value forest
        {
            Hash = Hash.value forestData.Hash
            Name = ForestName.value forestData.Name
            Created = forestData.Created
            LastModified = forestData.LastModified
            Status = ForestStatus.value forestData.Status
        }