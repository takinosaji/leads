from typing import Optional

from returns.result import safe

from leads.application_core.forests.models import ForestId
from leads.application_core.secondary_ports.forests import Forest, ForestPersister, ForestsRetriever, ForestRemover, \
    PersistedForestDto, ForestByNameRetriever, ForestByIdRetriever
from leads.secondary_adapters.mongodb_adapter.client_cache import MongoDbClientCache


__FORESTS_COLLECTION_NAME = "forests"


@safe
def __persist_forest(dep_client_cache: MongoDbClientCache,
                     forest: Forest) -> str:
    forests_collection = dep_client_cache.database.get_collection(__FORESTS_COLLECTION_NAME)
    forests_collection.insert_one(forest.model_dump())
    return forest.id
persist_forest: ForestPersister = __persist_forest


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
                    forest: Forest) -> None:
    forests_collection = dep_client_cache.database.get_collection(__FORESTS_COLLECTION_NAME)
    forests_collection.delete_one({"id": forest.id})
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


from leads.secondary_adapters.mongodb_adapter.configuration import MongoDbStorageConfiguration
# type MongoDBClientFactory = Callable[[MongoDbStorageConfiguration], MongoClient[dict]]
# def __create_mongodb_client(mongodb_configuration: MongoDbStorageConfiguration) -> MongoClient[dict]:
#     client = MongoClient(mongodb_configuration.connection_string)
#     return client
# create_mongodb_client: MongoDBClientFactory = __create_mongodb_client




