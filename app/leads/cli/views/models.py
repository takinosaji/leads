from enum import Enum
from typing import Optional
from pydantic import Field, BaseModel

from leads.application_core.secondary_ports.pydantic_models import model_config, ModelsMetadata


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
    sqlite_storage_connection_string: Optional[str] = Field(default="", json_schema_extra={ModelsMetadata.MASKED: True})
    mongodb_storage_connection_string: Optional[str] = Field(default="", json_schema_extra={ModelsMetadata.MASKED: True})
    mongodb_storage_database_name: Optional[str] = Field(default="")
    mongodb_timeout_ms: Optional[str] = Field(default="")
