namespace Leads.Shell

open System.CommandLine

type RootCommandBinder = RootCommand -> RootCommand
type SubCommandBinder = Command -> Command