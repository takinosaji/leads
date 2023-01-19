module Leads.Shell.Utilities

open System
open System.CommandLine


let createOption<'a> (name:string) description isRequired =
    let option = Option<'a>($"--{name}", description)
    option.AddAlias($"-{name[0]}")        
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