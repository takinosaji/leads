﻿namespace Leads.Core.Forests

open Leads.Core.Utilities.OptionExtensions

open Leads.Core.Forests.ForestDTO

module ForestsDTO =
    type ForestsDrivenDto = ForestDrivenDto list option
    type ForestsDrivingDto = ForestDrivingDto list option
    
open ForestsDTO

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
        | Some forests -> Some (forests |> List.map Forest.toOutputDto)
        | None -> None
    