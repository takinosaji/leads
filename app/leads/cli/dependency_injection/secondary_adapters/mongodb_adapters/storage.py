from partial_injector.partial_container import Container, FromContainer

from leads.cli.configuration.models import CliConfigurationCache
from leads.secondary_adapters.mongodb_adapter.client_cache import MongoDbClientCache
from leads.secondary_adapters.mongodb_adapter.configuration import MongoDbStorageConfiguration
from leads.secondary_adapters.mongodb_adapter.storage import LeadsStorageDatabase


def register_dependencies(container: Container) -> None:
    container.register_transient(FromContainer(CliConfigurationCache, lambda cache: cache.configuration.mongodb_storage_configuration),
                                 key=MongoDbStorageConfiguration)
    container.register_singleton_factory(lambda conf_cache: MongoDbClientCache(conf_cache),
                                         key=MongoDbClientCache,
                                         factory_args=[FromContainer(CliConfigurationCache)],
                                         condition=lambda conf: conf.connection_string and conf.database_name,
                                         condition_args=[FromContainer(MongoDbStorageConfiguration)],
                                         condition_ignore_not_satisfied=True)
    container.register_transient_factory(lambda client_cache, conf: client_cache.client[conf.database_name],
                                         factory_args=[FromContainer(MongoDbClientCache),
                                                       FromContainer(MongoDbStorageConfiguration)],
                                         key=LeadsStorageDatabase,
                                         condition=lambda conf: conf.connection_string and conf.database_name,
                                         condition_args=[FromContainer(MongoDbStorageConfiguration)],
                                         condition_ignore_not_satisfied=True)
    return None


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