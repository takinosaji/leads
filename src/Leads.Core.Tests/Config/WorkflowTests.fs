module Leads.Core.Tests.Config.WorkflowTests

open Leads.Core.Utilities.ConstrainedTypes
open Leads.Core.Utilities.Dependencies
open Leads.Core.Config.Workflows

open Xunit
open FsUnit.Xunit

let private fullConfiguration =
                Map.empty
                    .Add("working.dir", "path")
                    .Add("default.stream", "defaultStream")
                        
[<Theory>]
[<InlineData("working.dir", "path")>]
[<InlineData("default.stream", "defaultStream")>]
let ``When requesting the existing key expect return value`` requestedKey expectedValue =
    let stubConfigProvider: ConfigurationProvider =
        fun _ ->
            Ok(Some fullConfiguration) 
    reader {        
        return! getConfigWorkflow requestedKey
    } |> Reader.run {
        configProvider = stubConfigProvider
    } |> should equal (Ok(Some expectedValue))

[<Fact>]
let ``When requesting the unknown key expect error message`` () =
    let unknownKey = "unknownKey"
    let stubConfigProvider: ConfigurationProvider =
        fun _ ->
            Ok(Some fullConfiguration)
            
    reader {        
        return! getConfigWorkflow unknownKey
    } |> Reader.run {
        configProvider = stubConfigProvider
    } |> should equal (Error(ErrorText))