module Leads.Core.Tests.Config.GetConfigValueWorkflowTests

open Leads.SecondaryPorts.Config
open Leads.Utilities.Dependencies
open Leads.Core.Config.Workflows
open Leads.Core.Config.Services

open Xunit
open Swensen.Unquote

let private fullConfiguration =
                Map.empty
                    .Add("working.dir", "path")
                    .Add("default.forest", "defaultForest")
  
let private emptyConfiguration = Map.empty
                        
[<Theory>]
[<InlineData("working.dir", "path")>]
[<InlineData("default.forest", "defaultForest")>]
let ``When requesting the existing key expect return value`` requestedKey expectedValue =
    let stubConfigProvider: ConfigurationProvider =
        fun _ ->
            Ok(Some fullConfiguration) 
    let configValueOutput = (reader {        
        return! getConfigValueWorkflow requestedKey
    } |> Reader.run {
        provideAllowedConfigKeys = fun () -> [requestedKey]
        provideConfig = stubConfigProvider
    })
    
    let (Ok(Some text)) = configValueOutput
    text =! expectedValue

[<Fact>]
let ``When requesting the unknown key expect error message`` () =
    let unknownKey = "unknownKey"
    let stubAllowedKeyProvider = fun () -> []
    let stubConfigProvider: ConfigurationProvider =
        fun _ ->
            Ok(Some fullConfiguration)
            
    let configValueOutput = (reader {        
        return! getConfigValueWorkflow unknownKey
    } |> Reader.run {
        provideAllowedConfigKeys = stubAllowedKeyProvider
        provideConfig = stubConfigProvider
    })
    
    let (Error errorText) = configValueOutput
    errorText = $"Configuration key {unknownKey} is not allowed"
    
[<Fact>]
let ``When requesting the missing entry expect None`` () =
    let knownKey = "working.dir"
    let stubAllowedKeyProvider = fun () -> [knownKey]
    let stubConfigProvider: ConfigurationProvider =
        fun _ ->
            Ok(Some emptyConfiguration)
            
    let configValueOutput = (reader {        
        return! getConfigValueWorkflow knownKey
    } |> Reader.run {
        provideAllowedConfigKeys = stubAllowedKeyProvider
        provideConfig = stubConfigProvider
    })
    
    let (Ok value) = configValueOutput
    value =! None
    
[<Fact>]
let ``When requesting the known key and configuration file is missing expect None`` () =
    let knownKey = "working.dir"
    let stubAllowedKeyProvider = fun () -> [knownKey]
    let stubConfigProvider: ConfigurationProvider =
        fun _ ->
            Ok(None)
            
    let configValueOutput = (reader {        
        return! getConfigValueWorkflow knownKey
    } |> Reader.run {
        provideAllowedConfigKeys = stubAllowedKeyProvider
        provideConfig = stubConfigProvider
    })
    
    let (Ok value) = configValueOutput
    value =! None
    
[<Fact>]
let ``When requesting the known key and configuration provider throws expect error message`` () =
    let knownKey = "working.dir"
    let errorMessage = "Any error message"
    let stubAllowedKeyProvider = fun () -> [knownKey]
    let stubConfigProvider: ConfigurationProvider =
        fun _ ->
            Error(errorMessage)
            
    let configValueOutput = (reader {        
        return! getConfigValueWorkflow knownKey
    } |> Reader.run {
        provideAllowedConfigKeys = stubAllowedKeyProvider
        provideConfig = stubConfigProvider
    })
    
    let (Error errorText) = configValueOutput
    errorText =! errorMessage