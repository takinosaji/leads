namespace Leads.Core.Forests

open System
open Leads.SecondaryPorts.Forest

[<Flags>]
type ForestStatuses =
    | All = 0
    | Active = 1
    | Completed = 2
    | Archived = 4
    
module ForestStatuses = 
    let composeStatuses =
        fun allForests includeActiveOption includeCompletedForests includeArchivedForests ->
        match allForests, includeActiveOption, includeCompletedForests, includeArchivedForests with
        | true, _, _, _
        | false, true, true, true -> ForestStatuses.All
        
        | false, true, true, false -> ForestStatuses.Active ||| ForestStatuses.Completed        
        | false, false, true, true -> ForestStatuses.Completed ||| ForestStatuses.Archived    
        | false, true, false, true -> ForestStatuses.Active ||| ForestStatuses.Archived
        
        | false, false, true, false -> ForestStatuses.Completed
        | false, false, false, true -> ForestStatuses.Archived
        
        | false, true, false, false
        | _ -> ForestStatuses.Active
                  
    let internal toSIDto (forestStatuses: ForestStatuses) =
        let appendStatusIfFlagFound allFlags targetFlag valueToAdd accumulator =
            if allFlags &&& targetFlag = targetFlag then
                List.append accumulator [valueToAdd]
            else
                accumulator
                
        match forestStatuses with
        | ForestStatuses.All ->
            [ ForestStatus.Active; ForestStatus.Completed; ForestStatus.Archived ]
        | _ ->
            List.empty<ForestStatus>
            |> appendStatusIfFlagFound forestStatuses ForestStatuses.Active ForestStatus.Active
            |> appendStatusIfFlagFound forestStatuses ForestStatuses.Completed ForestStatus.Completed
            |> appendStatusIfFlagFound forestStatuses ForestStatuses.Archived ForestStatus.Archived