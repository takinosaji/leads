from typing import Callable, Any, Mapping

from pymongo.synchronous.database import Database
from pymongo import MongoClient

from leads.secondary_adapters.mongodb_adapter.configuration import MongoDbStorageConfiguration


type MongoDBClientFactory = Callable[[MongoDbStorageConfiguration], MongoClient[dict]]
type LeadsStorageDatabase = Database[Mapping[str, Any] | Any]


def __create_mongodb_client(mongodb_configuration: MongoDbStorageConfiguration) -> MongoClient[dict]:
    client = MongoClient(mongodb_configuration.connection_string)
    return client
create_mongodb_client: MongoDBClientFactory = __create_mongodb_client


