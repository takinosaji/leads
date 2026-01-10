from partial_injector.partial_container import Container, FromContainer
from structlog import BoundLogger

from leads.application_core.forests.services import create_forest, ForestCreator, updated_forest, ForestUpdater, \
    archive_forest, unarchive_forest, ForestArchiver, ForestUnarchiver, ForestsGetter, get_forests, delete_forest, \
    ForestDeleter, ForestByNameGetter, get_forest_by_name, ForestByIdGetter, get_forest_by_id

from leads.cli.configuration.factory import (load_n_cache_cli_configuration,
                                             CliConfigurationLoader,
                                             CliConfigurationSaver,
                                             CliConfigurationCache,
                                             save_cli_configuration)
from leads.cli.cli_logging import create_configured_logger
from leads.cli.dependency_injection import secondary_adapters


def get_container():
    container = Container()

    # Cli
    container.register_singleton_factory(lambda: CliConfigurationCache(), key=CliConfigurationCache)

    container.register_singleton(save_cli_configuration, key=CliConfigurationSaver)
    container.register_singleton(load_n_cache_cli_configuration, key=CliConfigurationLoader)
    container.register_singleton_factory(create_configured_logger, factory_args=[FromContainer(CliConfigurationLoader)], key=BoundLogger)

    # Application Core
    container.register_transient(create_forest, key=ForestCreator)
    container.register_transient(updated_forest, key=ForestUpdater)
    container.register_transient(archive_forest, key=ForestArchiver)
    container.register_transient(unarchive_forest, key=ForestUnarchiver)
    container.register_transient(get_forests, key=ForestsGetter)
    container.register_transient(get_forest_by_name, key=ForestByNameGetter)
    container.register_transient(get_forest_by_id, key=ForestByIdGetter)
    container.register_transient(delete_forest, key=ForestDeleter)

    # Secondary Adapter
    secondary_adapters.sqlite_adapters.storage.register_dependencies(container)
    secondary_adapters.mongodb_adapters.storage.register_dependencies(container)

    container.build()

    return container