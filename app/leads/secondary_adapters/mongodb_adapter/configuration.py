from typing import Optional

from pydantic import BaseModel, Field

from leads.application_core.secondary_ports.pydantic_models import model_config


class MongoDbStorageConfiguration(BaseModel):
    model_config = model_config

    connection_string: Optional[str] = Field(default=None)
    database_name: Optional[str] = Field(default=None)