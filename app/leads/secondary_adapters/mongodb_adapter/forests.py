from typing import Optional

from returns.result import safe

from leads.application_core.forests.models import ForestId
from leads.application_core.secondary_ports.forests import Forest, ForestInserter, ForestsRetriever, ForestRemover, \
    PersistedForestDto, ForestByNameRetriever, ForestByIdRetriever, ForestUpdater
from leads.secondary_adapters.mongodb_adapter.client_cache import MongoDbClientCache


__FORESTS_COLLECTION_NAME = "forests"


@safe
def __insert_forest(dep_client_cache: MongoDbClientCache,
                    forest: Forest) -> ForestId:
    forests_collection = dep_client_cache.database.get_collection(__FORESTS_COLLECTION_NAME)
    forests_collection.insert_one(forest.model_dump())
    return forest.id
insert_forest: ForestInserter = __insert_forest


@safe
def __update_forest(dep_client_cache: MongoDbClientCache,
                    forest: Forest) -> None:
    forests_collection = dep_client_cache.database.get_collection(__FORESTS_COLLECTION_NAME)
    forests_collection.update_one({"id": forest.id}, {"$set": forest.model_dump()})
    return None
update_forest: ForestUpdater = __update_forest


@safe
def __retrieve_forests(dep_client_cache: MongoDbClientCache,
                       include_archived: bool) -> list[PersistedForestDto]:
    forests_collection = dep_client_cache.database.get_collection(__FORESTS_COLLECTION_NAME)
    query_filter = {} if include_archived else {"is_archived": False}
    forest_dicts = forests_collection.find(query_filter)
    return forest_dicts.to_list()
retrieve_forests: ForestsRetriever = __retrieve_forests


@safe
def __remove_forest(dep_client_cache: MongoDbClientCache,
                    forest_id: ForestId) -> None:
    forests_collection = dep_client_cache.database.get_collection(__FORESTS_COLLECTION_NAME)
    forests_collection.delete_one({"id": forest_id})
    return None
remove_forest: ForestRemover = __remove_forest


@safe
def __retrieve_forest_by_name(dep_client_cache: MongoDbClientCache,
                              name: str) -> Optional[PersistedForestDto]:
    forests_collection = dep_client_cache.database.get_collection(__FORESTS_COLLECTION_NAME)
    forest_dict = forests_collection.find_one({"name": name})
    return forest_dict
retrieve_forest_by_name: ForestByNameRetriever = __retrieve_forest_by_name


@safe
def __retrieve_forest_by_id(dep_client_cache: MongoDbClientCache,
                            forest_id: ForestId) -> Optional[PersistedForestDto]:
    forests_collection = dep_client_cache.database.get_collection(__FORESTS_COLLECTION_NAME)
    forest_dict = forests_collection.find_one({"id": forest_id})
    return forest_dict
retrieve_forest_by_id: ForestByIdRetriever = __retrieve_forest_by_id


