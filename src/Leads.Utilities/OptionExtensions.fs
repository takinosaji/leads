module Leads.Utilities.OptionExtensions

type Option<'a> with
    static member toBoolean = function
        | Some (_: 'a) -> true
        | None -> false