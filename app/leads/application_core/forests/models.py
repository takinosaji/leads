from typing import Optional

from pydantic import BaseModel, Field

from leads.application_core.secondary_ports.pydantic_models import model_config


class Forest(BaseModel):
    model_config = model_config

    id: str = Field(...)
    name: str = Field(...)
    description: Optional[str] = Field(default=None)