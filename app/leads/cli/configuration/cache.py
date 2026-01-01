from leads.cli.configuration.models import CliConfiguration


class CliConfigurationCache:
    def __init__(self):
        self.configuration: CliConfiguration | None = None