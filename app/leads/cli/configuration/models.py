from typing import Optional

from pydantic import BaseModel, Field

from leads.application_core.secondary_ports.pydantic_models import model_config

type CliMinLogLevelValue = int
type CliMinLogLevelKey = str


class CliMinLogLevel(BaseModel):
    model_config = model_config

    key: CliMinLogLevelKey
    value: CliMinLogLevelValue


class RuntimeConfiguration(BaseModel):
    model_config = model_config

    min_log_level: str = Field(...)


class ContextConfiguration(BaseModel):
    model_config = model_config

    active_forest: Optional[str] = Field(default=None)


class CliConfiguration(BaseModel):
    model_config = model_config

    context_configuration: ContextConfiguration
    runtime_configuration: RuntimeConfiguration


