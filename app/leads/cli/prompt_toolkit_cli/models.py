from __future__ import annotations

from enum import Enum


class CliMode(str, Enum):
    NAVIGATION = "navigation"
    COMMAND = "command"
    EDITING = "editing"


class CliTab(str, Enum):
    CONFIGURATION = "Configuration"
    FORESTS = "Forests"
    TRAILS = "Trails"
