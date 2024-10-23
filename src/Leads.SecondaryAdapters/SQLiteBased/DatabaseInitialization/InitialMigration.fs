module Leads.SecondaryAdapters.SQLiteBased.DatabaseInitialization.InitialMigration

open FluentMigrator

[<Migration(1L)>]
type CreateForestsTable() =
    inherit Migration()

    override this.Up() =
        this.Create.Table("Forests")
            .WithColumn("Hash").AsString().PrimaryKey()
            .WithColumn("Name").AsString()
            .WithColumn("CreatedAt").AsDateTime()
            .WithColumn("UpdatedAt").AsDateTime()
            .WithColumn("Status").AsString()
        |> ignore

    override this.Down() =
        this.Delete.Table("Forests")
        |> ignore