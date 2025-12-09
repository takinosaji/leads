from partial_injector.partial_container import Container, FromContainer
from structlog import BoundLogger
from sversion.contracts import Version
from sversion.version_file_based import get_version

from leads.cli.main import main


def get_container():
    container = Container()

    # Primary Adapter
    container.register_instance(main)

    container.register_factory(get_version, args=[__file__], key=Version)

    # Application Core

    # Secondary Adapter

    container.build()

    return container