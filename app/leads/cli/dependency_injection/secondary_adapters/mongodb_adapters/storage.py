from partial_injector.partial_container import Container, FromContainer
from pymongo import MongoClient

from leads.cli.configuration.models import CliConfiguration
from leads.secondary_adapters.mongodb_adapter.configuration import MongoDbStorageConfiguration
from leads.secondary_adapters.mongodb_adapter.factories import create_mongodb_dto, MongoDBDtoCreator
from leads.secondary_adapters.mongodb_adapter.storage import create_mongodb_client, StorageDatabase, StorageCollection


def register_dependencies(container: Container) -> None:
    container.register_instance(FromContainer(CliConfiguration, lambda conf: conf.mongodb_storage_configuration),
                                key=MongoDbStorageConfiguration)
    container.register_instance(create_mongodb_dto, key=MongoDBDtoCreator)
    container.register_factory(create_mongodb_client,
                               args=[FromContainer(MongoDbStorageConfiguration)],
                               key=MongoClient)
    container.register_factory(lambda client, conf: client[conf.database_name],
                               args=[FromContainer(MongoClient), FromContainer(MongoDbStorageConfiguration)],
                               key=StorageDatabase)
    container.register_factory(lambda db, conf: db[conf.collection_name],
                               args=[FromContainer(StorageDatabase), FromContainer(MongoDbStorageConfiguration)],
                               key=StorageCollection)

    return None

    # container.register_instance(FromContainer(CliConfiguration, lambda conf: conf.monbodb_storage_configuration),
    #                             key=MongoDbStorageConfiguration,
    #                             condition=lambda conf: type(conf) == MongoDbStorageConfiguration,
    #                             condition_args=[FromContainer(StorageConfiguration)],
    #                             condition_ignore_not_satisfied=True)
    #
    # container.register_instance(create_mongodb_dto,
    #                             key=MongoDBDtoCreator,
    #                             condition=lambda conf: type(conf) == MongoDbStorageConfiguration,
    #                             condition_args=[FromContainer(StorageConfiguration)],
    #                             condition_ignore_not_satisfied=True)
    # container.register_factory(create_mongodb_client,
    #                            args=[FromContainer(MongoDbStorageConfiguration)],
    #                            key=MongoClient,
    #                            condition=lambda conf: type(conf) == MongoDbStorageConfiguration,
    #                            condition_args=[FromContainer(StorageConfiguration)],
    #                            condition_ignore_not_satisfied=True)
    # container.register_factory(lambda client, conf: client[conf.database_name],
    #                            args=[FromContainer(MongoClient), FromContainer(MongoDbStorageConfiguration)],
    #                            key=StorageDatabase,
    #                            condition=lambda conf: type(conf) == MongoDbStorageConfiguration,
    #                            condition_args=[FromContainer(StorageConfiguration)],
    #                            condition_ignore_not_satisfied=True)
    # container.register_factory(lambda db, conf: db[conf.collection_name],
    #                            args=[FromContainer(StorageDatabase), FromContainer(MongoDbStorageConfiguration)],
    #                            key=StorageCollection,
    #                            condition=lambda conf: type(conf) == MongoDbStorageConfiguration,
    #                            condition_args=[FromContainer(StorageConfiguration)],
    #                           condition_ignore_not_satisfied=True)