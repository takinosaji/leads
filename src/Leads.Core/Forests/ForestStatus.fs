namespace Leads.Core.Forests

open System
open Leads.Core.Utilities.ConstrainedTypes

type ForestStatus =
     private
    | Active
    | Completed
    | Archived

module ForestStatus =
    module DTO =            
        [<Flags>]
        type ForestStatusDto =
            | All = 0
            | Active = 1
            | Completed = 2
            | Archived = 4
    
    [<Literal>] let private ActiveForestStatus = "active"
    [<Literal>] let private CompletedForestStatus = "completed"
    [<Literal>] let private ArchivedForestStatus = "archived"
    // let private AllowedStatuses = [ ActiveForestStatus; CompletedForestStatus; ArchivedForestStatus ]
    
    let create (status:string) =
        match status.ToLower() with
        | ActiveForestStatus -> Ok ForestStatus.Active
        | CompletedForestStatus -> Ok ForestStatus.Completed
        | ArchivedForestStatus -> Ok ForestStatus.Archived
        | _ -> Error <| ErrorText $"Unrecognized status: {status}"
    
    let toDto = function
        | Active -> ActiveForestStatus
        | Completed -> CompletedForestStatus
        | Archived -> ArchivedForestStatus
        
        
    // let value (ForestStatus status) = status