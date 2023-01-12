module Leads.DrivenAdapters.ConfigAdapters

// open Leads.Core.LeadsConfigurations
// open Legivel.Serialization
// open System.Reflection
//
// let yamlFileConfigurationFactory: ConfigurationFactory =
//     fun _ ->
//         let filePath = $"{Assembly.GetEntryAssembly().Location}/config.yaml"
//
//         let c = Deserialize<Configuration> filePath
//         
//         
//         match Deserialize<Configuration> filePath with
//         | Success config.Data
//         | 