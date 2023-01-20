module Leads.Core.Models

open System

open Leads.Core.Utilities.ConstrainedTypes

module Hash =
    type Hash = private Hash of string 
    let create hash =
     createLimitedString (nameof(Hash)) Hash 10 hash           
    let value (Hash hash) = hash    



 

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

