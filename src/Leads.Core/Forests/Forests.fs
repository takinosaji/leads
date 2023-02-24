namespace Leads.Core.Forests

open Leads.Utilities.OptionExtensions
open Leads.Core.Forests.ForestDTO

module ForestsDto =          
    type ForestsPrimaryDto = ForestPrimaryOutputDto list option
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
    
    let toPrimaryDtoList (forests: Forests): ForestsPrimaryDto =
        match forests with
        | Some forests -> Some (forests |> List.map Forest.toPrimaryOutputDto)
        | None -> None
    