namespace Leads.DrivenPorts.Forest

open System
open Leads.DrivenPorts.Config.DTO

module DTO =        
    [<Flags>]
    type ForestStatusDto =
        | All = 0
        | Active = 1
        | Completed = 2
        | Archived = 4
    
    type ForestDrivenDto = { Hash: string; Name: string; Created: DateTime; LastModified: DateTime; Status: string }
           
    type ForestsDrivenDto = ForestDrivenDto list option

open DTO

type ForestsProvider = ConfigDrivenDto -> Result<ForestsDrivenDto, string>
type ForestAppender = ForestDrivenDto -> Result<ForestDrivenDto, string>

