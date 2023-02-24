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
    type ForestSecondaryOutputDto = ForestSecondaryInputDto
    
    type ForestsSecondaryInputDto = ForestSecondaryInputDto list option
    type ForestsSecondaryOutputDto = ForestSecondaryOutputDto list option
    

open DTO

type ForestsProvider = ValidConfigSecondaryInputDto -> Result<ForestSecondaryOutputDto, string>
type ForestAppender = ValidConfigSecondaryInputDto -> ForestSecondaryInputDto -> Result<ForestSecondaryOutputDto, string>
type ForestRetriever = ValidConfigSecondaryInputDto -> string -> Result<ForestSecondaryOutputDto option, string>
type ForestsFinder = ValidConfigSecondaryInputDto -> FindCriteriaDto -> Result<ForestSecondaryOutputDto list option, string>
