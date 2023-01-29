module Leads.Core.Utilities.ListExtensions

type List<'a> with
    static member iterp (everyPredicate: 'a -> unit) (allExceptLastPredicate: 'a -> unit) (list: 'a list): unit =
        List.iteri (fun currentIndex li ->
            match currentIndex with
            | i when i = List.length list ->                            
                everyPredicate li
            | _ ->
                everyPredicate li
                allExceptLastPredicate li            
            ) list