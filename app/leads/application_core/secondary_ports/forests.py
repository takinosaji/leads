from datetime import datetime
from typing import Optional, Callable
from pydantic import BaseModel, Field
from returns.result import Result

from leads.application_core.secondary_ports.pydantic_models import model_config


class Forest(BaseModel):
    model_config = model_config

    id: str = Field(...)
    name: str = Field(...)
    description: Optional[str] = Field(default=None)
    created_at: datetime = Field(...)
    updated_at: Optional[datetime] = Field(default=None)
    is_archived: bool = Field(default=False)


type ForestPersister = Callable[[Forest], Result]
type ForestRetriever = Callable[[bool], Result[list[Forest]]]
type ForestRemover = Callable[[Forest], Result]