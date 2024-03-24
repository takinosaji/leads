namespace Leads.Core.Forests

open System

open Leads.SecondaryPorts.Forest
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
        
        return Forest {
            Hash = hash
            Name = forestName
            Status = ForestStatus.Active
            Created = timeStamp
            LastModified = timeStamp
        }
    }

    let internal fromSODto (forestDto: ForestSODto) =
        result {
            let! forestHash = Hash.create forestDto.Hash
            let! forestName = ForestName.create forestDto.Name          
           
            return Forest {
                Hash = forestHash
                Name = forestName
                Status = forestDto.Status
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
           Status = forestData.Status
        }                
    
    let internal toPODto forest: ForestPODto =
        let forestData = value forest
        {
            Hash = Hash.value forestData.Hash
            Name = ForestName.value forestData.Name
            Created = forestData.Created
            LastModified = forestData.LastModified
            Status = forestData.Status
        }