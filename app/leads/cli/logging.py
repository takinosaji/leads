from typing import Optional

import structlog
from returns.result import Success

from leads.cli.configuration.factory import CliConfigurationLoader
from leads.cli.configuration.logging import get_default_logger


def create_configured_logger(dep_load_cli_configuration: CliConfigurationLoader) -> structlog.BoundLogger:
    configuration = dep_load_cli_configuration()

    match configuration:
        case Success(configuration):
            structlog.configure(
                processors=[
                    structlog.contextvars.merge_contextvars,
                    structlog.processors.add_log_level,
                    structlog.processors.StackInfoRenderer(),
                    structlog.dev.set_exc_info,
                    structlog.processors.TimeStamper(fmt="%Y-%m-%d %H:%M:%S", utc=True),
                    structlog.processors.JSONRenderer()
                ],
                wrapper_class=structlog.make_filtering_bound_logger(configuration.runtime_configuration.min_log_level.name),
                context_class=dict,
                logger_factory=structlog.PrintLoggerFactory(),
                cache_logger_on_first_use=False
            )

            return structlog.get_logger()
        case _:
            return get_default_logger()