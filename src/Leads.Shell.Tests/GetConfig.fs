module Leads.Shell.Tests.GetConfig

open Leads.Core.Utilities.Dependencies
open Xunit
open FsUnit.Xunit

let inline add x y = x + y

[<Fact>]
let ``When asking valid key expect the value``() =
    ()