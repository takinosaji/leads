from typing import Optional

from pydantic import BaseModel, Field

from leads.application_core.secondary_ports.pydantic_models import model_config, ModelsMetadata


class MongoDbStorageConfiguration(BaseModel):
    model_config = model_config

    connection_string: Optional[str] = Field(default=None, json_schema_extra={ModelsMetadata.MASKED: True})
    database_name: Optional[str] = Field(default=None)