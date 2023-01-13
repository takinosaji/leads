module Leads.Core.Config

type Configuration = {
    DefaultStream: Stream
    WorkingDir: string
}

type ConfigurationFactory = unit -> Configuration
