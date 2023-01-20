module Leads.Core.Streams.StreamName

open Leads.Core.Utilities.ConstrainedTypes

type StreamName = private StreamName of string 
let create name =
    createLimitedString (nameof(StreamName)) StreamName 15 name           
let value (StreamName name) = name    