namespace Leads.DrivenPorts.Forest

open System

module DTO =        
    [<Flags>]
    type ForestStatusDto =
        | All = 0
        | Active = 1
        | Completed = 2
        | Archived = 4
    
    type ForestDrivenDto = { Hash: string; Name: string; Created: DateTime; LastModified: DateTime; Status: string }
    
    type ValidForestDto = ForestDrivenDto
    type InvalidForestDto = { Forest: ForestDrivenDto; Error: string }
        
    type ForestDrivingDto =
        | ValidForestDto of ValidForestDto
        | InvalidForestDto of InvalidForestDto
        
    type ForestsDrivenDto = ForestDrivenDto list option
    type ForestsDrivingDto = ForestDrivingDto list option

open DTO

type ForestsProvider = unit -> Result<ForestsDrivenDto, string>
type ForestAppender = ForestDrivenDto -> Result<ForestDrivenDto, string>

