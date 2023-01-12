module Leads.Core.LeadsConfigurations

type Configuration = {
    DefaultStream: Stream
    WorkingDir: string
}

type ConfigurationFactory = unit -> Configuration
