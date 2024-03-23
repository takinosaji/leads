module Leads.Shell.Commands.Trail.Add

// open System
//
// open System.CommandLine
//
// open Leads.Utilities.Dependencies
//
// open Leads.Shell
// open Leads.Shell.Utilities
//
// open Spectre.Console
//
// let private printAddedTrail (trailDto: TrailPODto) =
//     let table = Table()
//     
//     table.Title <- TableTitle("New Trail")
//
//     table.AddColumn("Field")
//     table.AddColumn("Value")
//     
//     table.AddRow(nameof(trailDto.Hash), trailDto.Hash)
//     table.AddRow(nameof(trailDto.Name), trailDto.Name)
//     table.AddRow(nameof(trailDto.Status), trailDto.Status.ToString())
//     table.AddRow(nameof(trailDto.Created), trailDto.Created.ToString())
//     table.AddRow(nameof(trailDto.LastModified), trailDto.LastModified.ToString())
//            
//     AnsiConsole.Write(table);
//     
// let private handler name =
//     let addTrailResult = addTrailWorkflow name
//                           |> Reader.run addTrailEnvironment
//     
//     match addTrailResult with
//     | Ok forest ->
//         forest |> printAddedTrail
//     | Error errorText ->
//         errorText |> writeColoredLine ConsoleColor.Red
//     
// let appendTrailAddSubCommand: SubCommandAppender =
//     fun cmd ->        
//         let addForestSubCommand =
//             createCommand "add" "The add command creates the new trail"
//         let nameArgument =
//             createArgument<string> "name" "Set the unique trail theme"   
//         
//         addForestSubCommand.AddArgument nameArgument
//         
//         addForestSubCommand.SetHandler(handler, nameArgument)
//         
//         cmd.AddCommand addForestSubCommand
//         
//         cmd      