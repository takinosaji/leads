namespace Leads.Core.Forests

open System
open Leads.Core.Models
open ForestName

module DTO =
    type ForestDTO =
        | ValidForestDto of {| Hash: string; Name: string; CreationDateTime: DateTime; Status: string |}
        | InvalidForestDto of {| Serialized:string; Error:string |}
    type ForestsDto = ForestDTO list option

    [<Flags>]
    type ForestStatusDto =
        | All = 0
        | Active = 1
        | Completed = 2
        | Archived = 4
open DTO

type ForestData = {
    Hash: Hash
    Name: ForestName  
    CreationDateTime: DateTime
}
    
type ValidForest =
    | ActiveForest of ForestData
    | CompletedForest of ForestData
    | ArchivedForest of ForestData

type InvalidForest = {
    Serialized: string
    Error: string
}

type ValidatedForest = 
    | ValidForest of ValidForest
    | InvalidForest of InvalidForest
    
type Forest = private Forest of ValidatedForest
    
type Forests = Forest list option
       
module Forest =
    let value (Forest forest) = forest
    
    let toOutputDto (forest:Forest) :ForestDTO  =
        let forestValue = value forest
        
        match forestValue with
        | ValidForest validForest ->
            match validForest with
            | ActiveForest activeForest ->
                ValidForestDto {|
                   Hash = Hash.value activeForest.Hash
                   Name = ForestName.value activeForest.Name
                   CreationDateTime = activeForest.CreationDateTime
                   Status = nameof(ActiveForest)
                |}
            | CompletedForest completedForest ->
                ValidForestDto {|
                   Hash = Hash.value completedForest.Hash
                   Name = ForestName.value completedForest.Name
                   CreationDateTime = completedForest.CreationDateTime
                   Status = nameof(CompletedForest)
                |}
            | ArchivedForest archivedForest ->
                ValidForestDto {|
                   Hash = Hash.value archivedForest.Hash
                   Name = ForestName.value archivedForest.Name
                   CreationDateTime = archivedForest.CreationDateTime
                   Status = nameof(ArchivedForest)
                |}
        | InvalidForest invalidForest ->
            InvalidForestDto {|
                Serialized = invalidForest.Serialized
                Error = invalidForest.Error
            |}
            
        