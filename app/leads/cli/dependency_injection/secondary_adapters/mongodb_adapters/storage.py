from partial_injector.partial_container import Container, FromContainer

from leads.application_core.secondary_ports.forests import ForestStorageInserter, ForestsStorageRetriever, ForestStorageRemover, \
    ForestByNameStorageRetriever, ForestByIdStorageRetriever, ForestStorageUpdater
from leads.cli.configuration.models import CliConfigurationCache
from leads.secondary_adapters.mongodb_adapter.client_cache import MongoDbClientCache
from leads.secondary_adapters.mongodb_adapter.configuration import MongoDbStorageConfiguration
from leads.secondary_adapters.mongodb_adapter.forests import insert_forest_into_storage, retrieve_forests_from_storage, remove_forest_from_storage, \
    retrieve_forest_by_name_from_storage, retrieve_forest_by_id, update_forest_in_storage
from leads.secondary_adapters.sqlite_adapter.configuration import SQLiteStorageConfiguration


def register_dependencies(container: Container) -> None:
    mongo_condition = dict(
        condition=lambda m_conf, s_conf: (m_conf and
                                          m_conf.connection_string and
                                          m_conf.database_name and
                                          m_conf.timeout_ms and
                                          not s_conf.connection_string),
        condition_args=[FromContainer(MongoDbStorageConfiguration), FromContainer(SQLiteStorageConfiguration)],
        throw_if_condition_not_satisfied_for_all=True
    )

    container.register_transient(FromContainer(CliConfigurationCache, lambda cache: cache.configuration.mongodb_storage_configuration),
                                 key=MongoDbStorageConfiguration)
    container.register_singleton_factory(lambda conf: MongoDbClientCache(conf),
                                         key=MongoDbClientCache,
                                         factory_args=[FromContainer(MongoDbStorageConfiguration)])
    container.register_transient(insert_forest_into_storage, key=ForestStorageInserter, **mongo_condition)
    container.register_transient(update_forest_in_storage, key=ForestStorageUpdater, **mongo_condition)
    container.register_transient(retrieve_forests_from_storage, key=ForestsStorageRetriever, **mongo_condition)
    container.register_transient(retrieve_forest_by_name_from_storage, key=ForestByNameStorageRetriever, **mongo_condition)
    container.register_transient(retrieve_forest_by_id, key=ForestByIdStorageRetriever, **mongo_condition)
    container.register_transient(remove_forest_from_storage, key=ForestStorageRemover, **mongo_condition)

    return None

    #
    # container.register_singleton_factory(lambda conf: MongoDbClientCache(conf),
    #                                      key=MongoDbClientCache,
    #                                      factory_args=[FromContainer(MongoDbStorageConfiguration)],
    #                                      condition=lambda m_conf, s_conf: m_conf.connection_string and m_conf.database_name and not s_conf.connection_string,
    #                                      condition_args=[FromContainer(MongoDbStorageConfiguration), FromContainer(SQLiteStorageConfiguration)],
    #                                      throw_if_condition_not_satisfied_for_all=True)

    # container.register_transient(FromContainer(CliConfigurationCache, lambda cache: cache.configuration.mongodb_storage_configuration),
    #                              key=MongoDbStorageConfiguration)
    # container.register_transient(create_mongodb_dto,
    #                              key=MongoDBDtoCreator,
    #                              condition=lambda conf: conf.connection_string and conf.database_name,
    #                              condition_args=[FromContainer(MongoDbStorageConfiguration)],
    #                              condition_ignore_not_satisfied=True)
    # container.register_transient_factory(create_mongodb_client,
    #                                      key=MongoClient,
    #                                      factory_args=[FromContainer(MongoDbStorageConfiguration, lambda conf: conf.connection_string)],
    #                                      condition=lambda conf: conf.connection_string and conf.database_name,
    #                                      condition_args=[FromContainer(MongoDbStorageConfiguration)],
    #                                      condition_ignore_not_satisfied=True)
    # container.register_transient_factory(lambda client, conf: client[conf.database_name],
    #                                      factory_args=[FromContainer(MongoClient),
    #                                                    FromContainer(MongoDbStorageConfiguration)],
    #                                      key=LeadsStorageDatabase,
    #                                      condition=lambda conf: conf.connection_string and conf.database_name,
    #                                      condition_args=[FromContainer(MongoDbStorageConfiguration)],
    #                                      condition_ignore_not_satisfied=True)