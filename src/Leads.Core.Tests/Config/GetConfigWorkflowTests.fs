module Leads.Core.Tests.Config.GetConfigWorkflowTests

open Leads.Core.Utilities.ConstrainedTypes
open Leads.Core.Utilities.Dependencies
open Leads.Core.Config.Workflows

open Xunit
open Swensen.Unquote

let private fullConfiguration =
                Map.empty
                    .Add("working.dir", "path")
                    .Add("default.stream", "defaultStream")
                    
let stubConfigApplier _ _ _: Result<unit, ErrorText> = Ok () 
  
let private emptyConfiguration = Map.empty
                        
[<Theory>]
[<InlineData("working.dir", "path")>]
[<InlineData("default.stream", "defaultStream")>]
let ``When requesting the existing key expect return value`` requestedKey expectedValue =
    let stubConfigProvider: ConfigurationProvider =
        fun _ ->
            Ok(Some fullConfiguration) 
    let configValueOutput = (reader {        
        return! getConfigValueWorkflow requestedKey
    } |> Reader.run {
        configFilePath = "any"
        provideConfig = stubConfigProvider
        applyConfigValue = stubConfigApplier
    })
    
    let (Ok(Some text)) = configValueOutput
    text =! expectedValue

[<Fact>]
let ``When requesting the unknown key expect error message`` () =
    let unknownKey = "unknownKey"
    let stubConfigProvider: ConfigurationProvider =
        fun _ ->
            Ok(Some fullConfiguration)
            
    let configValueOutput = (reader {        
        return! getConfigValueWorkflow unknownKey
    } |> Reader.run {
        configFilePath = "any"
        provideConfig = stubConfigProvider
        applyConfigValue = stubConfigApplier
    })
    
    let (Error errorText) = configValueOutput
    errorText =! "ConfigKey's value must be in range of allowed values"
    
[<Fact>]
let ``When requesting the missing entry expect None`` () =
    let knownKey = "working.dir"
    let stubConfigProvider: ConfigurationProvider =
        fun _ ->
            Ok(Some emptyConfiguration)
            
    let configValueOutput = (reader {        
        return! getConfigValueWorkflow knownKey
    } |> Reader.run {
        configFilePath = "any"
        provideConfig = stubConfigProvider
        applyConfigValue = stubConfigApplier
    })
    
    let (Ok value) = configValueOutput
    value =! None
    
[<Fact>]
let ``When requesting the known key and configuration file is missing expect None`` () =
    let knownKey = "working.dir"
    let stubConfigProvider: ConfigurationProvider =
        fun _ ->
            Ok(None)
            
    let configValueOutput = (reader {        
        return! getConfigValueWorkflow knownKey
    } |> Reader.run {
        configFilePath = "any"
        provideConfig = stubConfigProvider
        applyConfigValue = stubConfigApplier
    })
    
    let (Ok value) = configValueOutput
    value =! None
    
[<Fact>]
let ``When requesting the known key and configuration provider throws expect error message`` () =
    let knownKey = "working.dir"
    let errorMessage = "Any error message"
    let stubConfigProvider: ConfigurationProvider =
        fun _ ->
            Error(ErrorText errorMessage)
            
    let configValueOutput = (reader {        
        return! getConfigValueWorkflow knownKey
    } |> Reader.run {
        configFilePath = "any"
        provideConfig = stubConfigProvider
        applyConfigValue = stubConfigApplier
    })
    
    let (Error errorText) = configValueOutput
    errorText =! errorMessage