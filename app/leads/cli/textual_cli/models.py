from enum import Enum


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
