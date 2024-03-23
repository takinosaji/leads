module Leads.Core.Tests.Config.SetConfigValueWorkflowTests

open Leads.Utilities.Dependencies
open Leads.Core.Config.Workflows

open Xunit
open Swensen.Unquote

[<Fact>]
let ``When setting the value for an existing key expect Ok`` () =
    let knownKey = "working.dir"
    let newValue = "newPath"
    let stubConfigEnvironment: SetConfigEnvironment = {
        provideAllowedConfigKeys = fun () -> [knownKey]
        applyConfigValue = fun _ _ -> Ok()
    }
    let configValueOutput = (reader {
        return! setConfigValueWorkflow knownKey newValue
    } |> Reader.run stubConfigEnvironment)

    configValueOutput =! Ok ()

[<Fact>]
let ``When setting the value for an unknown key expect error message`` () =
    let unknownKey = "unknownKey"
    let newValue = "newPath"
    let stubConfigEnvironment: SetConfigEnvironment = {
        provideAllowedConfigKeys = fun () -> []
        applyConfigValue = fun _ _ -> Ok()
    }
    let configValueOutput = (reader {
        return! setConfigValueWorkflow unknownKey newValue
    } |> Reader.run stubConfigEnvironment)

    let (Error errorText) = configValueOutput
    errorText =! $"Configuration key {unknownKey} is not allowed"

[<Fact>]
let ``When setting the value and configuration provider throws expect error message`` () =
    let knownKey = "working.dir"
    let newValue = "newPath"
    let errorMessage = "Any error message"
    let stubConfigEnvironment: SetConfigEnvironment = {
        provideAllowedConfigKeys = fun () -> [knownKey]
        applyConfigValue = fun _ _ -> Error(errorMessage)
    }
    let configValueOutput = (reader {
        return! setConfigValueWorkflow knownKey newValue
    } |> Reader.run stubConfigEnvironment)

    configValueOutput =! Error errorMessage