module Leads.SecondaryAdapters.SQLiteBased.ForestAdapters

open Leads.SecondaryPorts.Forest.DTO
open Fumble

let private listForests
    (connectionString: string)    
    : Result<ForestSODto list option, string> =
     try
        let forests =
            connectionString
            |> Sql.connect
            |> Sql.query "SELECT * FROM Forests"
            |> Sql.execute (fun read ->
                {
                    Hash = read.string "Hash"
                    Name = read.string "Name"
                    CreatedAt = read.dateTime "CreatedAt"
                    UpdatedAt = read.dateTime "UpdatedAt"
                    Status = read.string "Status"
                })
        match forests with
        | Ok(forests) -> Ok(Some(forests))
        | Error(error) -> Error(error.ToString())
     with excp ->
        Error excp.Message