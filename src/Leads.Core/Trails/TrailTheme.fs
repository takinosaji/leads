namespace Leads.Core.Trails

open Leads.Utilities.ConstrainedTypes

type TrailTheme = private TrailTheme of string

module TrailTheme =
    let create theme =
        createLimitedString (nameof(TrailTheme)) TrailTheme 30 theme           
    let value (TrailTheme name) = name    