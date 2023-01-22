module Leads.Core.Forests.ForestName

open Leads.Core.Utilities.ConstrainedTypes

type ForestName = private ForestName of string 
let create name =
    createLimitedString (nameof(ForestName)) ForestName 15 name           
let value (ForestName name) = name    