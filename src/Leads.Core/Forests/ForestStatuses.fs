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