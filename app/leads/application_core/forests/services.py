from datetime import datetime, timezone
from typing import Callable
from nanoid import generate
from returns.result import Result, safe

from leads.application_core.secondary_ports.forests import Forest, ForestPersister, ForestRetriever, ForestRemover


type ForestId = str
type ForestName = str
type ForestDescription = str
type ForestCreator = Callable[[ForestName, ForestDescription], Result[Forest]]
type ForestEditor = Callable[[Forest, ForestName, ForestDescription], Result]
type ForestArchiver = Callable[[Forest], Result]
type ForestDeleter = Callable[[Forest], Result]
type ForestUnarchiver = Callable[[Forest], Result]
type ForestGetter = Callable[[bool], Result[list[Forest]]]


@safe
def __get_forests(dep_forests_retriever: ForestRetriever,
                  include_archived: bool) -> list[Forest]:
    forests = dep_forests_retriever(include_archived).unwrap()
    return forests
get_forests: ForestGetter = __get_forests


@safe
def __create_forest(dep_persist_forest: ForestPersister,
                    name: ForestName,
                    description: ForestDescription) -> Forest:
    forest_id = generate()
    moment_of_time = datetime.now(timezone.utc)
    forest = Forest(id=forest_id,
                    name=name,
                    description=description,
                    created_at=moment_of_time,
                    updated_at=moment_of_time)
    dep_persist_forest(forest)
    return forest
create_forest: ForestCreator = __create_forest

@safe
def __edit_forest(forest: Forest, name: ForestName, description: ForestDescription) -> None:
    forest.name = name
    forest.description = description
    forest.updated_at = datetime.now(timezone.utc)
edit_forest: ForestEditor = __edit_forest

@safe
def __archive_forest(forest: Forest) -> None:
    forest.updated_at = datetime.now(timezone.utc)
    forest.is_archived = True
archive_forest: ForestArchiver = __archive_forest

@safe
def __unarchive_forest(forest: Forest) -> None:
    forest.updated_at = datetime.now(timezone.utc)
    forest.is_archived = False
unarchive_forest: ForestUnarchiver = __unarchive_forest

@safe
def __delete_forest(dep_remove_forest: ForestRemover,
                    forest: Forest) -> None:
    dep_remove_forest(forest)
delete_forest: ForestDeleter = __delete_forest
