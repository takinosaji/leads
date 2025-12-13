from __future__ import annotations

from enum import Enum


class CliMode(str, Enum):
    NAVIGATION = "navigation"
    EDITING = "editing"

