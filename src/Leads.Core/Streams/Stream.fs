namespace Leads.Core.Streams

open System
open Leads.Core.Models.Hash
open StreamName

module Stream =
    type StreamData = {
        Hash: Hash
        Name: StreamName  
        CreationDateTime: DateTime
    }

    type Stream = 
        | ActiveStream of StreamData
        | CompletedStream of StreamData
        | ArchivedStream of StreamData