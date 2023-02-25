namespace Leads.SecondaryPorts.Forest

open System
open Leads.SecondaryPorts.Config.DTO

module DTO =     
    type TextCriteria =
        | All
        | ContainsText of string
        
    type FindCriteriaDto = {
        text: TextCriteria
        statuses: string list
    }
    
    type ForestSecondaryInputDto = { Hash: string; Name: string; Created: DateTime; LastModified: DateTime; Status: string }
    type ForestSODto = ForestSecondaryInputDto
    
    type ForestsSecondaryInputDto = ForestSecondaryInputDto list option
    type ForestsSecondaryOutputDto = ForestSODto list option
    

open DTO

type ForestAppender = ValidConfigSecondaryInputDto -> ForestSecondaryInputDto -> Result<ForestSODto, string>
type ForestRetriever = ValidConfigSecondaryInputDto -> string -> Result<ForestSODto option, string>
type ForestsFinder = ValidConfigSecondaryInputDto -> FindCriteriaDto -> Result<ForestSODto list option, string>
type ForestUpdater = ValidConfigSecondaryInputDto -> ForestSecondaryInputDto -> Result<ForestSODto, string>
