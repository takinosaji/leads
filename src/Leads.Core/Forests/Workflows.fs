module Leads.Core.Forests.Workflows

open Leads.Core.Forests.DTO
open Leads.Core.Utilities.ConstrainedTypes
open Leads.Core.Utilities.Dependencies

type StreamsProvider = unit -> Result<ForestsDto, ErrorText>
// type ConfigurationValueApplier = ConfigKey -> ConfigValue -> Result<unit, ErrorText>
type StreamEnvironment = {
    provideStreams: StreamsProvider
}

type ListStreamsWorkflow = ForestStatusDto -> Reader<StreamEnvironment, Result<ForestsDto, string>>
// let listForestsWorkflow: ListStreamsWorkflow =
//     