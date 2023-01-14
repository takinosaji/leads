namespace Leads.Shell

open System.CommandLine

type RootCommandAppender = RootCommand -> RootCommand
type SubCommandAppender = Command -> Command