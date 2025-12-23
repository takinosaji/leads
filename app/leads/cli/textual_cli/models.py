from dataclasses import dataclass
from enum import Enum

from leads.cli.configuration.models import CliConfiguration


class CliMode(str, Enum):
    NAVIGATION = "navigation"
    COMMAND = "command"
    EDITING = "editing"


class CliPanels(str, Enum):
    MENU = "menu"
    CONTENT = "content"
    STATE = "state"
    TITLE = "title"
    COMMAND = "command"
    NOTIFICATION = "notification"


class CliTab(str, Enum):
    CONFIGURATION = "Configuration"
    FORESTS = "Forests"
    TRAILS = "Trails"


@dataclass
class FlatConfiguration:
    def __init__(self, cli_configuration: CliConfiguration):
        self.min_log_level = cli_configuration.runtime_configuration.min_log_level.name
        self.active_forest = cli_configuration.context_configuration.active_forest or ""