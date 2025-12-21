from typing import Optional

from pydantic import BaseModel, Field, field_validator

from leads.application_core.secondary_ports.pydantic_models import model_config
from leads.cli.configuration.logging import LogLevel


class RuntimeConfiguration(BaseModel):
    model_config = model_config

    min_log_level: LogLevel = Field(...)

    @field_validator("min_log_level", mode="before")
    def parse_color(cls, v):
        if isinstance(v, str) and v in LogLevel.__members__:
            return LogLevel[v]
        return v



class ContextConfiguration(BaseModel):
    model_config = model_config

    active_forest: Optional[str] = Field(default=None)


class CliConfiguration(BaseModel):
    model_config = model_config

    context_configuration: ContextConfiguration
    runtime_configuration: RuntimeConfiguration


