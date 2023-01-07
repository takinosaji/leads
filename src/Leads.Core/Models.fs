namespace Leads.Core

open System
open FSharp.FGL
//open ConstrainedTypes

// module Tag =
//     type Tag = private Tag of string 
//     let create tag =
//         createLimitedString ("Tag") Tag 5 tag           
//     let value (Tag name) = name    

 

type Tag = Tag of string

type Lead =
    {
        Text: string
        Hash: string
        Tags: Tag list
        DateTime: DateTime
        Links: TO DO ... think of simple links
    }
    
   
type Theme = Theme of string
    

    
type Trail = {
    Leads: Lead list
    Theme: Theme
    Hash: string
}

type Stream = {
    Name: String
    Trails = 
}