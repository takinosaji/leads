from typing import Optional
from pydantic import BaseModel, Field, field_validator, model_validator

from leads.application_core.configuration import ContextConfiguration, ContextConfigurationGetter
from leads.application_core.secondary_ports.pydantic_models import model_config
from leads.cli.configuration.configuration_logging import LogLevel
from leads.secondary_adapters.mongodb_adapter.configuration import MongoDbStorageConfiguration, \
    MongoDbStorageConfigurationGetter
from leads.secondary_adapters.sqlite_adapter.configuration import SQLiteStorageConfiguration


class RuntimeConfiguration(BaseModel):
    model_config = model_config

    min_log_level: str = Field(...)

    @field_validator("min_log_level", mode="before")
    def parse_log_level(cls, value: str) -> str:
        if isinstance(value, LogLevel):
            return value.name
        if isinstance(value, str) and value in LogLevel.__members__:
            return value
        raise ValueError(f"Invalid min_log_level '{value}'. Allowed: {', '.join(LogLevel.__members__.keys())}")


class CliConfiguration(BaseModel):
    model_config = model_config

    context_configuration: ContextConfiguration = Field(...)
    runtime_configuration: RuntimeConfiguration = Field(...)
    mongodb_storage_configuration: Optional[MongoDbStorageConfiguration] = Field(None)
    sqlite_storage_configuration: Optional[SQLiteStorageConfiguration] = Field(None)

    @model_validator(mode="after")
    def check_storage_config(self):
        if not self.mongodb_storage_configuration and not self.sqlite_storage_configuration:
            raise ValueError("Either mongodb_storage_configuration or sqlite_storage_configuration must be present.")
        return self


class CliConfigurationCache:
    def __init__(self):
        self.configuration: Optional[CliConfiguration] = None

    @property
    def mongodb_storage_configuration_getter(self) -> MongoDbStorageConfigurationGetter:
        def getter():
            return self.configuration.mongodb_storage_configuration
        return getter

    @property
    def context_configuration(self) -> ContextConfigurationGetter:
        def getter():
            return self.configuration.context_configuration
        return getter