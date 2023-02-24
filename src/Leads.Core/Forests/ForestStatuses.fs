namespace Leads.Core.Forests

open System

[<Flags>]
type ForestStatuses =
    | All = 0
    | Active = 1
    | Completed = 2
    | Archived = 4
    
module ForestStatuses = 
    let composeStatuses =
        fun allForests includeCompletedForests includeArchivedForests ->
        match allForests, includeCompletedForests, includeArchivedForests with
        | true, _, _ -> ForestStatuses.All
        | false, true, true -> ForestStatuses.Completed ||| ForestStatuses.Archived
        | false, true, false -> ForestStatuses.Completed
        | false, false, true -> ForestStatuses.Archived
        | _ -> ForestStatuses.Active
                  
    let internal toStatusesSecondaryDto (forestStatuses: ForestStatuses) =
        let appendStatusIfFlagFound allFlags targetFlag valueToAdd accumulator =
            if allFlags &&& targetFlag = targetFlag then
                List.append accumulator [valueToAdd]
            else
                accumulator
        
        let active = ForestStatus.createActive() |> ForestStatus.value
        let completed = ForestStatus.createCompleted() |> ForestStatus.value
        let archived = ForestStatus.createArchived() |> ForestStatus.value
        
        match forestStatuses with
        | ForestStatuses.All ->
            [ active; completed; archived ]
        | _ ->
            List.empty<string>
            |> appendStatusIfFlagFound forestStatuses ForestStatuses.Active active
            |> appendStatusIfFlagFound forestStatuses ForestStatuses.Completed completed
            |> appendStatusIfFlagFound forestStatuses ForestStatuses.Archived archived