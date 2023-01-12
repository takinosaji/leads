module Leads.Core.Tests.GetConfig

open Xunit
open FsUnit.Xunit

let inline add x y = x + y

[<Fact>]
let ``When 2 is added to 2 expect 4``() =
    add 2 2 |> should equal 4 
