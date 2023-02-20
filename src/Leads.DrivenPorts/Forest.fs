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
    
    type ForestDrivenInputDto = { Hash: string; Name: string; Created: DateTime; LastModified: DateTime; Status: string }
    type ForestDrivenOutputDto = ForestDrivenInputDto
    
    type ForestsDrivenInputDto = ForestDrivenInputDto list option
    type ForestsDrivenOutputDto = ForestDrivenOutputDto list option
    

open DTO

type ForestsProvider = ValidConfigDrivenInputDto -> Result<ForestsDrivenOutputDto, string>
type ForestAppender = ValidConfigDrivenInputDto -> ForestDrivenInputDto -> Result<ForestDrivenOutputDto, string>
type ForestRetriever = ValidConfigDrivenInputDto -> string -> Result<ForestDrivenOutputDto option, string>
type ForestFinder = ValidConfigDrivenInputDto -> string -> Result<ForestDrivenOutputDto list option, string>
