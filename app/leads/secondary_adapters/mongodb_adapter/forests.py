from returns.result import safe

from leads.application_core.secondary_ports.forests import Forest, ForestPersister, ForestRetriever, ForestRemover
from leads.secondary_adapters.mongodb_adapter.client_cache import MongoDbClientCache

__FORESTS_COLLECTION_NAME = "forests"

@safe
def __persist_forest(dep_client_cache: MongoDbClientCache,
                     forest: Forest) -> None:
    forests_collection = dep_client_cache.database.get_collection(__FORESTS_COLLECTION_NAME)
    forests_collection.insert_one(forest.model_dump())
    return None
persist_forest: ForestPersister = __persist_forest


@safe
def __retrieve_forests(dep_client_cache: MongoDbClientCache,
                       include_archived: bool) -> list[Forest]:
    forests_collection = dep_client_cache.database.get_collection(__FORESTS_COLLECTION_NAME)
    query_filter = {} if include_archived else {"is_archived": False}
    forest_dicts = forests_collection.find(query_filter)
    forests = [Forest.model_validate(forest_dict) for forest_dict in forest_dicts]
    return forests
retrieve_forests: ForestRetriever = __retrieve_forests

@safe
def __remove_forest(dep_client_cache: MongoDbClientCache,
                    forest: Forest) -> None:
    forests_collection = dep_client_cache.database.get_collection(__FORESTS_COLLECTION_NAME)
    forests_collection.delete_one({"id": forest.id})
    return None
remove_forest: ForestRemover = __remove_forest



from leads.secondary_adapters.mongodb_adapter.configuration import MongoDbStorageConfiguration
# type MongoDBClientFactory = Callable[[MongoDbStorageConfiguration], MongoClient[dict]]
# def __create_mongodb_client(mongodb_configuration: MongoDbStorageConfiguration) -> MongoClient[dict]:
#     client = MongoClient(mongodb_configuration.connection_string)
#     return client
# create_mongodb_client: MongoDBClientFactory = __create_mongodb_client




