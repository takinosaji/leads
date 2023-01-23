module Leads.Core.Forests.Workflows

open Leads.Core.Config
open Leads.Core.Config.ConfigKey
open Leads.Core.Config.Workflows
open Leads.Core.Utilities.Result

open Leads.Core.Forests.DTO
open Leads.Core.Utilities.ConstrainedTypes
open Leads.Core.Utilities.Dependencies

type ForestsProvider = string -> Result<ForestsDto, ErrorText>
type ForestEnvironment = {
    configFilePath: string
    defaultWorkingDirPath: string
    provideConfig: ConfigurationProvider
    provideForests: ForestsProvider
}

let private toGetConfigEnvironment forestEnvironment = {
    configFilePath = forestEnvironment.configFilePath
    provideConfig =  forestEnvironment.provideConfig
}

// TODO: Write unit tests
type ListForestsWorkflow = ForestStatusDto -> Reader<ForestEnvironment, Result<ForestsDto, string>>
let listForestsWorkflow: ListForestsWorkflow =
    fun statusDto -> reader {
        let! environment = Reader.ask
        let! getWorkingDirPathResult = getConfigInternalWorkflow WorkingDirKey
                                    |> Reader.withEnv toGetConfigEnvironment
        return result {
            let! workingDirPath = getWorkingDirPathResult
            
            let! defaultWorkingDirPath = environment.provideForests environment.defaultWorkingDirPath
            
          
        } |> Result.mapError errorTextToString     
    }