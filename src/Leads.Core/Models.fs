module Leads.Core.Models

open System

open System.Security.Cryptography
open Leads.Core.Utilities.ConstrainedTypes

type Hash = private Hash of string 
module Hash =
    let create hash =
        createLimitedString (nameof(Hash)) Hash 10 hash           
    let value (Hash hash) = hash
    
    let newRandom() =
        Guid.NewGuid().ToByteArray()
        |> BitConverter.ToString
        |> create

let createDateTime dateTimeString =
    try
        Ok (DateTime.Parse dateTimeString)
    with
    | excp -> Error (ErrorText excp.Message)


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

