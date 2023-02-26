namespace Leads.SecondaryPorts.Forest

open System
open Leads.SecondaryPorts.Config.DTO

module DTO =       
    type TextCriteria =
        | Any
        | Contains of string
        | Exact of string
        
    type AndFindCriteria = {
        Name: TextCriteria
        Hash: TextCriteria
        Statuses: string list
    }
    
    type OrFindCriteria = AndFindCriteria list
    
    type ForestSIDto = { Hash: string; Name: string; Created: DateTime; LastModified: DateTime; Status: string }
    type ForestSODto = ForestSIDto
    
    type ForestsSecondaryInputDto = ForestSIDto list option
    type ForestsSecondaryOutputDto = ForestSODto list option    
open DTO

type ForestAppender = ValidConfigSIDto -> ForestSIDto -> Result<ForestSODto, string>
type ForestRetriever = ValidConfigSIDto -> string -> Result<ForestSODto option, string>
type ForestsFinder = ValidConfigSIDto -> OrFindCriteria -> Result<ForestSODto list option, string>
type ForestUpdater = ValidConfigSIDto -> ForestSIDto -> Result<unit, string>
