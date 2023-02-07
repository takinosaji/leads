namespace Leads.Core.Forests

open Leads.Utilities.ConstrainedTypes

type ForestName = private ForestName of string

module ForestName =
    let create name =
        createLimitedString (nameof(ForestName)) ForestName 15 name           
    let value (ForestName name) = name    