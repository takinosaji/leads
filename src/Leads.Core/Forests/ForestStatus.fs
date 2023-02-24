namespace Leads.Core.Forests

open System
open Leads.Utilities.ConstrainedTypes

type ForestStatus = private ForestStatus of string

module ForestStatus =    
    [<Literal>]
    let private ActiveForestStatus = "active"
    [<Literal>]
    let private CompletedForestStatus = "completed"
    [<Literal>]
    let private ArchivedForestStatus = "archived"
    
    let internal create (status:string) =
        createPredefinedString
             (nameof ForestStatus)
             ForestStatus status
             [ActiveForestStatus; CompletedForestStatus; ArchivedForestStatus] 
       
    let value (ForestStatus status) = status 
    
    let internal createActive() =
        let (Ok status) = create ActiveForestStatus
        status
        
    let internal createArchived() =
        let (Ok status) = create ArchivedForestStatus
        status
                
    let internal createCompleted() =
        let (Ok status) = create CompletedForestStatus
        status