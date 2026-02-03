from typing import Optional, Callable
from pydantic import BaseModel, Field


from leads.application_core.secondary_ports.pydantic_models import model_config


type ContextConfigurationGetter = Callable[[], ContextConfiguration]


class ContextConfiguration(BaseModel):
    model_config = model_config

    active_forest: Optional[str] = Field(default=None)