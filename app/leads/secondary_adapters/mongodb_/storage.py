from typing import Callable

from pymongo.synchronous.collection import Collection
from pymongo.synchronous.database import Database
from pymongo import MongoClient

from leads.secondary_adapters.mongodb_.configuration import MongoDbStorageConfiguration

type MongoDBClientFactory = Callable[[MongoDbStorageConfiguration], MongoClient[dict]]
type StorageCollection = Collection[dict]
type StorageDatabase = Database[Collection[dict]]


def __create_mongodb_client(mongodb_configuration: MongoDbStorageConfiguration) -> MongoClient[dict]:
    client = MongoClient(mongodb_configuration.connection_string)
    return client
create_mongodb_client: MongoDBClientFactory = __create_mongodb_client