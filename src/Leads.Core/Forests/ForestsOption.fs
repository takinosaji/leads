namespace Leads.Core.Forests

open Leads.Utilities.OptionExtensions
open Leads.Core.Forests.Forest.DTO

type ForestsOption = Forest list option
    
module ForestsOption =
    module DTO =          
        type ForestsOptionPODto = ForestPODto list option
        type ValidForestsOptionPODto = ValidForestPODto list option
    open DTO
    
    let exists name = function
        | Some (forests: Forest list) ->
            List.tryFind (fun forest ->
                match Forest.value forest with
                | ValidForestCase validForest ->
                    validForest.Name = name
                | _ -> false
                ) forests
            |> Option.toBoolean
        | None -> false
        
    let filterOutInvalid (forestsOption: ForestsOption) =
        match forestsOption with
        | Some forests ->
            match List.choose (fun forest ->
                       match Forest.value forest with
                       | ValidForestCase vf -> Some vf
                       | _ -> None
                      ) forests with
            | [] -> None
            | filteredList -> (Some filteredList)
        | None -> None      
    
    let toPrimaryOutputDtoList (forests: ForestsOption): ForestsOptionPODto =
        match forests with
        | Some forests -> Some (forests |> List.map Forest.toPrimaryOutputDto)
        | None -> None
        
    let toValidForestPODtoList (validForests: ValidForest list option): ValidForestsOptionPODto =
        match validForests with
        | Some forests -> Some (forests |> List.map Forest.toValidForestOutputDto)
        | None -> None
    