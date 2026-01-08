from partial_injector.partial_container import Container, FromContainer

from leads.cli.configuration.models import CliConfigurationCache
from leads.secondary_adapters.sqlite_adapter.configuration import SQLiteStorageConfiguration


def register_dependencies(container: Container) -> None:
    container.register_transient(FromContainer(CliConfigurationCache, lambda cache: cache.configuration.sqlite_storage_configuration),
                                     key=SQLiteStorageConfiguration)
    # container.register_transient_factory(create_sqlite_client,
    #                                      key=SqliteClient,
    #                                      factory_args=[FromContainer(SQLiteStorageConfiguration, lambda conf: conf.connection_string)],
    #                                      condition=lambda conf: conf.connection_string,
    #                                      condition_args=[FromContainer(SQLiteStorageConfiguration)],
    #                                      condition_ignore_not_satisfied=True)
    return None
