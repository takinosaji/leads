 module Leads.Shell.Commands.Forest.Complete
//
// open System
//
// open System.CommandLine
//
// open Leads.Core.Forests.ForestDto
// open Leads.Utilities.Dependencies
//
// open Leads.SecondaryPorts.Forest.DTO
// open Leads.Core.Forests.Workflows
//
// open Leads.Shell
// open Leads.Shell.Utilities
// open Leads.Shell.Commands.Forest.Environment
// open Spectre.Console
//
// let private printCompletedForest (forestDto: ValidForestDto) =
//     
//     let table = Table()
//     
//     table.Title <- TableTitle("Completed Forest")
//
//     table.AddColumn("Field")
//     table.AddColumn("Value")
//     
//     table.AddRow(nameof(forestDto.Hash), forestDto.Hash)
//     table.AddRow(nameof(forestDto.Name), forestDto.Name)
//     table.AddRow(nameof(forestDto.Status), forestDto.Status)
//     table.AddRow(nameof(forestDto.Created), forestDto.Created.ToString())
//     table.AddRow(nameof(forestDto.LastModified), forestDto.LastModified.ToString())
//            
//     AnsiConsole.Write(table);
//     
// let private handler name =
//     reader {       
//         let! completeForestResult = completeForestWorkflow nameOrHash
//         
//         match completeForestResult with
//         | Ok forest ->
//             forest |> printCompletedForest
//         | Error errorText ->
//             errorText |> writeColoredLine ConsoleColor.Red
//     } |> Reader.run completeForestEnvironment
//     
// let completeForestAddSubCommand: SubCommandAppender =
//     fun cmd ->        
//         let completeForestSubCommand =
//             createCommand "add" "The complete command completes existing forest"
//         let nameOrHashArgument =
//             createArgument<string> "name" "Provide the unique forest hash or name"   
//         
//         completeForestSubCommand.AddArgument nameOrHashArgument
//         
//         completeForestSubCommand.SetHandler(handler, nameOrHashArgument)
//         
//         cmd.AddCommand completeForestSubCommand
//         
//         cmd      