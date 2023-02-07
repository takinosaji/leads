namespace Leads.Core.Forests

open System
open Leads.Utilities.ConstrainedTypes

type ForestStatus = // TODO: rewrite - this stinks we should be fine with simple enum or add proper smart constructor
     private
    | Active
    | Completed
    | Archived

module ForestStatus =    
    [<Literal>]
    let private ActiveForestStatus = "active"
    [<Literal>]
    let private CompletedForestStatus = "completed"
    [<Literal>]
    let private ArchivedForestStatus = "archived"
    
    let create (status:string) =
        match status.ToLower() with
        | ActiveForestStatus -> Ok ForestStatus.Active
        | CompletedForestStatus -> Ok ForestStatus.Completed
        | ArchivedForestStatus -> Ok ForestStatus.Archived
        | _ -> Error <| ErrorText $"Unrecognized status: {status}"
                
    let value = function
        | Active -> ActiveForestStatus
        | Completed -> CompletedForestStatus
        | Archived -> ArchivedForestStatus
    
    let createActive() =
        create ActiveForestStatus