from datetime import datetime
from typing import Optional, Callable
from pydantic import BaseModel, Field
from pydantic.dataclasses import dataclass
from returns.result import Result

from leads.application_core.forests.services import ForestId
from leads.application_core.secondary_ports.pydantic_models import model_config


type PersistedForestDto = dict


@dataclass
class NewForestDto:
    name: str
    description: str


@dataclass
class UpdateForestDto:
    id: str
    name: str
    description: str
    is_archived: bool


class Forest(BaseModel):
    model_config = model_config

    id: str = Field(..., min_length=1)
    name: str = Field(..., min_length=1)
    description: Optional[str] = Field(default=None)
    created_at: datetime = Field(...)
    updated_at: Optional[datetime] = Field(default=None)
    is_archived: bool = Field(default=False)


type ForestStorageInserter = Callable[[Forest], Result[ForestId]]
type ForestStorageUpdater = Callable[[Forest], Result]
type ForestsStorageRetriever = Callable[[bool], Result[list[PersistedForestDto]]]
type ForestByNameStorageRetriever = Callable[[str], Result[Optional[PersistedForestDto]]]
type ForestByIdStorageRetriever = Callable[[ForestId], Result[Optional[PersistedForestDto]]]
type ForestStorageRemover = Callable[[ForestId], Result]