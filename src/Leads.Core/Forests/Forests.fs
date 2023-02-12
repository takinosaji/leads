namespace Leads.Core.Forests

open Leads.Utilities.OptionExtensions
open Leads.Core.Forests.ForestDto

module ForestsDto =          
    type ForestsDrivingDto = ForestDrivingDto list option
open ForestsDto

type Forests = Forest list option
    
module Forests =
    let exists name = function
        | Some (forests: Forest list) ->
            List.tryFind (fun forest ->
                match Forest.value forest with
                | ValidForest validForest ->
                    validForest.Name = name
                | _ -> false
                ) forests
            |> Option.toBoolean
        | None -> false
    
    let toDrivingDtoList (forests: Forests): ForestsDrivingDto =
        match forests with
        | Some forests -> Some (forests |> List.map Forest.toDrivingDto)
        | None -> None
    