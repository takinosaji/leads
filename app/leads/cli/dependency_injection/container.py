from partial_injector.partial_container import Container, FromContainer
from structlog import BoundLogger

from leads.cli.configuration.factory import load_cli_configuration, CliConfigurationLoader, CliConfigurationSaver, \
    save_cli_configuration
from leads.cli.cli_logging import create_configured_logger


def get_container():
    container = Container()

    # CLI
    container.register_instance(save_cli_configuration, key=CliConfigurationSaver)
    container.register_instance(load_cli_configuration, key=CliConfigurationLoader)
    container.register_factory(create_configured_logger, args=[FromContainer(CliConfigurationLoader)], key=BoundLogger)

    # Application Core

    # Secondary Adapter

    container.build()

    return container