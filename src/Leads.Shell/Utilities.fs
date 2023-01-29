module Leads.Shell.Utilities

open System
open System.CommandLine

let createCommand name description =
    let command = Command(name, description)
    command.AddAlias($"-{name[0]}")   
    
    command

let createOption<'a> (name:string) description isRequired =
    let option = Option<'a>($"--{name}", description)
    option.AddAlias($"-{name[0]}")        
    option.IsRequired <- isRequired
    
    option
    
let createOptionWithAlias<'a> (name:string) alias description isRequired =
    let option = Option<'a>($"--{name}", description)
    option.AddAlias($"-{alias}")        
    option.IsRequired <- isRequired
    
    option
 
let addOption<'a> (name:string) description isRequired (cmd:Command) =
    let option = Option<'a>($"--{name}", description)
    option.AddAlias($"-{name[0]}")        
    option.IsRequired <- isRequired
    
    cmd.AddOption option
    
    cmd    

let createArgument<'a> (name:string) description =
    Argument<'a>(name, description)               
    
let addArgument<'a> (name:string) description (cmd:Command) =
    let argument = Argument<'a>(name, description)               
    cmd.AddArgument argument
    
    cmd
   

let writeColoredLine color (text:string) =
    Console.ForegroundColor <- color
    Console.WriteLine text
    Console.ResetColor()
    
let writeErrorLine = writeColoredLine ConsoleColor.Red

let writeEmptyLine (_:unit) =
    Console.WriteLine()