namespace Leads.Core.Forests

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
    
    let internal createActive() = ForestStatus ActiveForestStatus
        
    let internal createArchived() = ForestStatus ArchivedForestStatus
                
    let internal createCompleted() = ForestStatus CompletedForestStatus