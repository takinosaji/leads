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
  
let private emptyConfiguration = Map.empty
                        
[<Theory>]
[<InlineData("working.dir", "path")>]
[<InlineData("default.stream", "defaultStream")>]
let ``When requesting the existing key expect return value`` requestedKey expectedValue =
    let stubConfigProvider: ConfigurationProvider =
        fun _ ->
            Ok(Some fullConfiguration) 
    let configValueOutput = (reader {        
        return! getConfigWorkflow requestedKey
    } |> Reader.run {
        configProvider = stubConfigProvider
    })
    
    let (Ok(Some text)) = configValueOutput
    text |> should equal expectedValue

[<Fact>]
let ``When requesting the unknown key expect error message`` () =
    let unknownKey = "unknownKey"
    let stubConfigProvider: ConfigurationProvider =
        fun _ ->
            Ok(Some fullConfiguration)
            
    let configValueOutput = (reader {        
        return! getConfigWorkflow unknownKey
    } |> Reader.run {
        configProvider = stubConfigProvider
    })
    
    let (Error(ErrorText text)) = configValueOutput
    text |> should equal "ConfigKey's value must be in range of allowed values"
    
[<Fact>]
let ``When requesting the missing entry expect None`` () =
    let unknownKey = "working.dir"
    let stubConfigProvider: ConfigurationProvider =
        fun _ ->
            Ok(Some emptyConfiguration)
            
    let configValueOutput = (reader {        
        return! getConfigWorkflow unknownKey
    } |> Reader.run {
        configProvider = stubConfigProvider
    })
    
    let (Ok value) = configValueOutput
    value |> should equal None