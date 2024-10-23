namespace Leads.Core.Forests

open Leads.Utilities.OptionExtensions
open Leads.Core.Forests.Forest.DTO

open System

type ForestsOption = Forest list option
    
module Forests =
    module DTO =          
        type ForestsOptionPODto = ForestPODto list option
    open DTO
    
    let exists name = function
        | Some (forests: Forest list) ->
            forests
            |> List.map (fun f -> Forest.value f)
            |> List.tryFind (fun forest ->
                forest.Name = name) 
            |> Option.toBoolean
        | None -> false
            
    let extractNamesString forests =
        let forestData = List.map (fun f -> Forest.value f) forests
        String.Join(", ", forestData |> List.map (_.Name))
            
    let toOptionPODtoList (forests: ForestsOption): ForestsOptionPODto =
        match forests with
        | Some forests -> Some (forests |> List.map Forest.toPODto)
        | None -> None
        
    