from partial_injector.partial_container import Container, FromContainer
from structlog import BoundLogger

from leads.cli.configuration.factory import create_configuration
from leads.cli.configuration.logging import create_configured_logger
from leads.cli.configuration.models import CliConfiguration


def get_container():
    container = Container()

    # CLI
    container.register_factory(create_configuration, key=CliConfiguration)
    container.register_factory(create_configured_logger, args=[FromContainer(CliConfiguration)], key=BoundLogger)

    # Application Core

    # Secondary Adapter

    container.build()

    return container