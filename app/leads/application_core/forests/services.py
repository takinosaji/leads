from datetime import datetime, timezone
from typing import Callable, Optional
from nanoid import generate
from pydantic.dataclasses import dataclass
from returns.pointfree import bind
from returns.result import Result, safe, Success
from returns.pipeline import flow

from leads.application_core.forests.models import ForestName, ForestId, ForestUpdatedAt
from leads.application_core.secondary_ports.forests import Forest, ForestStorageInserter, ForestsStorageRetriever, ForestStorageRemover, \
    PersistedForestDto, ForestByIdStorageRetriever, ForestByNameStorageRetriever, NewForestDto, UpdateForestDto, ForestStorageUpdater, \
    ForestStorageArchiver, ForestStorageUnarchiver


type ForestCreator = Callable[[NewForestDto], Result[Forest]]
type ForestEditor = Callable[[Forest], Result]
type ForestArchiver = Callable[[ForestId], Result[Forest]]
type ForestDeleter = Callable[[ForestId], Result]
type ForestUnarchiver = Callable[[ForestId], Result[Forest]]
type ForestsGetter = Callable[[bool], Result[list[Forest]]]
type ForestByNameGetter = Callable[[ForestName], Result[Optional[Forest]]]
type ForestByIdGetter = Callable[[ForestId], Result[Optional[Forest]]]


@safe
def __get_forests(dep_retrieve_forests: ForestsStorageRetriever,
                  include_archived: bool) -> list[Forest]:
    @safe
    def create_forests(dtos: list[PersistedForestDto]):
        return [Forest.model_validate(forest_dict) for forest_dict in dtos]

    forests = (dep_retrieve_forests(include_archived)
                             .bind(create_forests)
                             .unwrap())
    return forests
get_forests: ForestsGetter = __get_forests


@safe
def __get_forest_by_name(dep_retrieve_forest_by_name: ForestByNameStorageRetriever,
                         name: ForestName) -> Optional[Forest]:
    @safe
    def from_persisted_dto(dto: Optional[PersistedForestDto]) -> Optional[Forest]:
        return Forest.model_validate(dto) if dto is not None else None

    forest = flow(name,
                  dep_retrieve_forest_by_name,
                  bind(from_persisted_dto)).unwrap()

    return forest
get_forest_by_name: ForestByNameGetter = __get_forest_by_name


@safe
def __get_forest_by_id(dep_retrieve_forest_by_id: ForestByIdStorageRetriever,
                       forest_id: ForestId) -> Optional[Forest]:
     @safe
     def from_persisted_dto(dto: Optional[PersistedForestDto]) -> Optional[Forest]:
          return Forest.model_validate(dto) if dto is not None else None

     forest = (dep_retrieve_forest_by_id(forest_id)
                  .bind(from_persisted_dto)
                  .unwrap())

     return forest
get_forest_by_id: ForestByIdGetter = __get_forest_by_id


def __create_forest(dep_persist_forest: ForestStorageInserter,
                    dep_get_forest_by_name: ForestByNameGetter,
                    dep_get_forest_by_id: ForestByIdGetter,
                    dto: NewForestDto) -> Forest:
    @dataclass
    class ForestCreationState:
        forest: Optional[Forest] = None
        existing_forest: Optional[Forest] = None
        persisted_id: Optional[ForestId] = None
        persisted_dto: Optional[PersistedForestDto] = None

    @safe
    def from_new_dto(new_forest_dto: NewForestDto) -> ForestCreationState:
        fid = generate()
        moment_of_time = datetime.now(timezone.utc)
        forest = Forest(id=fid,
                        name=new_forest_dto.name,
                        description=new_forest_dto.description,
                        created_at=moment_of_time,
                        updated_at=moment_of_time)
        return ForestCreationState(forest=forest)

    @safe
    def get_forest_if_exists(state: ForestCreationState) -> ForestCreationState:
        state.existing_forest = dep_get_forest_by_name(state.forest.name).unwrap()
        return state

    @safe
    def get_persisted_forest(state: ForestCreationState) -> ForestCreationState:
        state.persisted_dto = dep_get_forest_by_id(state.persisted_id).unwrap().model_dump()
        return state

    @safe
    def handle_forest_exists(state: ForestCreationState) -> ForestCreationState:
        if state.existing_forest is not None:
            raise Exception(f"Forest with name '{state.existing_forest.name}' already exists.")
        return state

    @safe
    def insert_new_forest(state: ForestCreationState) -> ForestCreationState:
        state.persisted_id = dep_persist_forest(state.forest).unwrap()
        return state

    @safe
    def persist_dto_to_model(state: ForestCreationState) -> Forest:
        return Forest.model_validate(state.persisted_dto)

    return flow(dto,
        from_new_dto,
        bind(get_forest_if_exists),
        bind(handle_forest_exists),
        bind(insert_new_forest),
        bind(get_persisted_forest),
        bind(persist_dto_to_model)
    )
create_forest: ForestCreator = __create_forest


def __edit_forest(dep_get_forest_by_id: ForestByIdGetter,
                  dep_update_forest: ForestStorageUpdater,
                  dto: UpdateForestDto) -> Forest:
    @dataclass
    class ForestUpdateState:
        update_dto: Optional[UpdateForestDto] = None
        existing_forest: Optional[Forest] = None
        updated_forest: Optional[Forest] = None

    @safe
    def get_forest_if_exists(update_dto: UpdateForestDto) -> ForestUpdateState:
        state = ForestUpdateState(update_dto=update_dto)
        state.existing_forest = dep_get_forest_by_id(state.update_dto.id).unwrap()
        return state

    @safe
    def handle_forest_doesnt_exist(state: ForestUpdateState) -> ForestUpdateState:
        if state.existing_forest is None:
            raise Exception(f"Forest with id '{state.update_dto.id}' doesnt exist.")
        return state

    @safe
    def update_forest(state: ForestUpdateState) -> ForestUpdateState:
        state.updated_forest = Forest(id=state.existing_forest.id,
                                      name=state.update_dto.name,
                                      description=state.update_dto.description,
                                      is_archived=state.update_dto.is_archived,
                                      updated_at=datetime.now(timezone.utc),
                                      created_at=state.existing_forest.created_at)
        dep_update_forest(state.updated_forest).unwrap()
        return state

    return flow(dto,
        get_forest_if_exists,
        bind(handle_forest_doesnt_exist),
        bind(update_forest),
        bind(lambda state: Success(state.updated_forest))
    )
edit_forest: ForestEditor = __edit_forest


def __archive_forest(dep_get_forest_by_id: ForestByIdGetter,
                     dep_archive_forest_in_storage: ForestStorageArchiver,
                     forest_id: ForestId) -> Forest:
    @dataclass
    class ForestArchiveState:
        forest_id: Optional[ForestId] = None
        existing_forest: Optional[Forest] = None

    @safe
    def get_forest_if_exists(fid: ForestId) -> ForestArchiveState:
        state = ForestArchiveState(forest_id=fid)
        state.existing_forest = dep_get_forest_by_id(fid).unwrap()
        return state

    @safe
    def handle_forest_doesnt_exist(state: ForestArchiveState) -> ForestArchiveState:
        if state.existing_forest is None:
            raise Exception(f"Forest with id '{state.forest_id}' doesnt exist.")
        return state

    @safe
    def handle_already_archived(state: ForestArchiveState) -> ForestArchiveState:
        if state.existing_forest.is_archived:
            raise Exception(f"Forest with id '{state.forest_id}' is already archived.")
        dep_archive_forest_in_storage(state.forest_id,
                                      datetime.now(timezone.utc)
                                      ).unwrap()
        return state

    @safe
    def return_forest(state: ForestArchiveState) -> Forest:
        return state.existing_forest

    return flow(forest_id,
                get_forest_if_exists,
                bind(handle_forest_doesnt_exist),
                bind(handle_already_archived),
                bind(return_forest))
archive_forest: ForestArchiver = __archive_forest



def __unarchive_forest(dep_get_forest_by_id: ForestByIdGetter,
                       dep_unarchive_forest_in_storage: ForestStorageUnarchiver,
                       forest_id: ForestId) -> Forest:
    @dataclass
    class ForestUnarchiveState:
        forest_id: Optional[ForestId] = None
        existing_forest: Optional[Forest] = None

    @safe
    def get_forest_if_exists(fid: ForestId) -> ForestUnarchiveState:
        state = ForestUnarchiveState(forest_id=fid)
        state.existing_forest = dep_get_forest_by_id(fid).unwrap()
        return state

    @safe
    def handle_forest_doesnt_exist(state: ForestUnarchiveState) -> ForestUnarchiveState:
        if state.existing_forest is None:
            raise Exception(f"Forest with id '{state.forest_id}' doesnt exist.")
        return state

    @safe
    def handle_not_archived(state: ForestUnarchiveState) -> ForestUnarchiveState:
        if not state.existing_forest.is_archived:
            raise Exception(f"Forest with id '{state.forest_id}' is not archived.")
        dep_unarchive_forest_in_storage(state.forest_id,
                                        datetime.now(timezone.utc)).unwrap()
        return state

    @safe
    def return_forest(state: ForestUnarchiveState) -> Forest:
        return state.existing_forest

    return flow(forest_id,
                get_forest_if_exists,
                bind(handle_forest_doesnt_exist),
                bind(handle_not_archived),
                bind(return_forest))
unarchive_forest: ForestUnarchiver = __unarchive_forest


def __delete_forest(dep_remove_forest: ForestStorageRemover,
                    forest_id: ForestId) -> None:
    return flow(forest_id,
                dep_remove_forest)
delete_forest: ForestDeleter = __delete_forest
