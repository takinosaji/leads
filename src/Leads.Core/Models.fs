module Leads.Core.Models

open System

type Error = Error of string
//open ConstrainedTypes

// module Tag =
//     type Tag = private Tag of string 
//     let create tag =
//         createLimitedString ("Tag") Tag 5 tag           
//     let value (Tag name) = name    

 

type Tag = Tag of string

type Link = {
        TargetLeadHash: String
        TargetTrailHash: String
        ConnectionDateTime: DateTime
    }

type Lead = {
        Text: string
        Hash: string
        Tags: Tag list
        CreationDateTime: DateTime
        Links: Link list
    }
    
type CompletionStatus =
    | Finished
    | Archived
   
type Theme = Theme of string
        
type TrailData = {
        Leads: Lead list
        Theme: Theme
        Hash: string
        CreationDateTime: DateTime
    }
type Trail =
    | ActiveTrail of TrailData
    | CompletedTrail of TrailData
    | ArchivedTrail of TrailData

type StreamData = {
        Name: String  
        CreationDateTime: DateTime
        Trails: Trail list
    }
type Stream =
    | ActiveStream of StreamData
    | CompletedStream of StreamData
    | ArchivedStream of StreamData