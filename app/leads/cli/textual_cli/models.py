from enum import Enum
from typing import Optional

from pydantic import BaseModel, Field, field_validator

from leads.application_core.secondary_ports.pydantic_models import model_config
from leads.cli.configuration.logging import LogLevel


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


class FlatConfiguration(BaseModel):
    model_config = model_config

    min_log_level: str = Field(...)
    active_forest: Optional[str] = Field(default="")

    @field_validator("min_log_level", mode="before")
    def validate_min_log_level(cls, value: str):
        if isinstance(value, LogLevel):
            return value.name
        if isinstance(value, str):
            upper = value.upper()
            if upper in LogLevel.__members__:
                return upper
        raise ValueError(
            f"Invalid min_log_level '{value}'. "
            f"Allowed values: {', '.join(LogLevel.__members__.keys())}"
        )
