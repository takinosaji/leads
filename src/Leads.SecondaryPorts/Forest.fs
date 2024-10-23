namespace Leads.SecondaryPorts.Forest

open System
open Leads.SecondaryPorts.Config.DTO

type ForestStatus =
    | Active
    | Completed
    | Archived

module DTO =       
    type TextCriteria =
        | Any
        | Contains of string
        | Exact of string
        
    type AndFindCriteria = {
        Name: TextCriteria
        Hash: TextCriteria
        Statuses: ForestStatus list
    }
    
    type OrFindCriteria = AndFindCriteria list    
    type ForestSIDto = { Hash: string; Name: string; CreatedAt: DateTime; UpdatedAt: DateTime; Status: ForestStatus }
    type ForestSODto = { Hash: string; Name: string; CreatedAt: DateTime; UpdatedAt: DateTime; Status: string }
    
    type ForestsSIDto = ForestSIDto list option
    type ForestsSODto = ForestSODto list option    
open DTO

type ForestAppender = ValidConfigSIDto -> ForestSIDto -> Result<ForestSODto, string>
type ForestRetriever = ValidConfigSIDto -> string -> Result<ForestSODto option, string>
type ForestsFinder = ValidConfigSIDto -> OrFindCriteria -> Result<ForestSODto list option, string>
type ForestUpdater = ValidConfigSIDto -> ForestSIDto -> Result<unit, string>
type ForestDeleter = ValidConfigSIDto -> ForestSIDto -> Result<unit, string>
