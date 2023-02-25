module Leads.Core.Forests.Services

open Leads.Core.Config.Services
open Leads.SecondaryPorts.Config
open Leads.SecondaryPorts.Forest
open Leads.SecondaryPorts.Forest.DTO
open Leads.Utilities.Result
open Leads.Utilities.ConstrainedTypes
open Leads.Utilities.Dependencies

open Leads.Core.Config

type FindForestEnvironment = {
    provideConfig: ConfigurationProvider
    findForests: ForestsFinder
}

let private toGetConfigEnvironment (forestEnvironment:FindForestEnvironment) = {
    provideConfig =  forestEnvironment.provideConfig
}

type internal FindForests = FindCriteriaDto -> Reader<FindForestEnvironment, Result<ForestsOption, ErrorText>>
let internal findForests: FindForests =
    fun findCriteria -> reader {
        let! environment = Reader.ask
        let! getConfigResult = getConfig()
                                    |> Reader.withEnv toGetConfigEnvironment
        return result {
            let! config = getConfigResult
            let configDto = config |> Configuration.toValidSecondaryInputDto
            
            let! unvalidatedForestsOption =
                findCriteria
                |> environment.findForests configDto 
                |> Result.mapError stringToErrorText
                
            match unvalidatedForestsOption with
            | Some unvalidatedForests ->                
                return unvalidatedForests
                        |> List.map Forest.fromSecondaryOutputDto
                        |> Some
            | None -> return None            
        }    
    }