import logging
import structlog
from typing import Optional

from .models import CliConfiguration, CliMinLogLevelKey, CliMinLogLevel


DEFAULT_MIN_LOG_LEVEL_KEY = 'INFO'


log_levels = {
    'DEBUG': logging.DEBUG,
    'INFO': logging.INFO,
    'WARNING': logging.WARNING,
    'ERROR': logging.ERROR,
    'CRITICAL': logging.CRITICAL
}


def get_default_logger() -> structlog.BoundLogger:
    structlog.configure(
        processors=[
            structlog.contextvars.merge_contextvars,
            structlog.processors.add_log_level,
            structlog.processors.StackInfoRenderer(),
            structlog.dev.set_exc_info,
            structlog.processors.TimeStamper(fmt="%Y-%m-%d %H:%M:%S", utc=True),
            structlog.processors.JSONRenderer()
        ],
        wrapper_class=structlog.make_filtering_bound_logger(logging.NOTSET),
        context_class=dict,
        logger_factory=structlog.PrintLoggerFactory(),
        cache_logger_on_first_use=False
    )
    return structlog.get_logger()


def create_configured_logger(dep_configuration: CliConfiguration) -> structlog.BoundLogger:
    min_log_level = __resolve_min_log_level_or_default(dep_configuration.runtime_configuration.min_log_level).value

    structlog.configure(
        processors=[
            structlog.contextvars.merge_contextvars,
            structlog.processors.add_log_level,
            structlog.processors.StackInfoRenderer(),
            structlog.dev.set_exc_info,
            structlog.processors.TimeStamper(fmt="%Y-%m-%d %H:%M:%S", utc=True),
            structlog.processors.JSONRenderer()
        ],
        wrapper_class=structlog.make_filtering_bound_logger(min_log_level),
        context_class=dict,
        logger_factory=structlog.PrintLoggerFactory(),
        cache_logger_on_first_use=False
    )

    return structlog.get_logger()


def __resolve_min_log_level_or_default(min_log_level_key: Optional[CliMinLogLevelKey]) -> CliMinLogLevel:
    match min_log_level_key:
        case None:
            min_log_level_key = DEFAULT_MIN_LOG_LEVEL_KEY
            min_log_level_value = log_levels[DEFAULT_MIN_LOG_LEVEL_KEY]
        case level if level in log_levels:
            min_log_level_key = level
            min_log_level_value = log_levels[level]
        case _:
            raise ValueError(f'Invalid log level: {min_log_level_key}')

    return CliMinLogLevel(key=min_log_level_key, value=min_log_level_value)