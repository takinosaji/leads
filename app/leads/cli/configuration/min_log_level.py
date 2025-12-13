import logging
from typing import Optional

from .models import CliMinLogLevelValue, CliMinLogLevelKey, CliMinLogLevel

DEFAULT_MIN_LOG_LEVEL_KEY = 'INFO'

log_levels = {
    'DEBUG': logging.DEBUG,
    'INFO': logging.INFO,
    'WARNING': logging.WARNING,
    'ERROR': logging.ERROR,
    'CRITICAL': logging.CRITICAL
}

def resolve_min_log_level_or_default(min_log_level_key: Optional[CliMinLogLevelKey]) -> CliMinLogLevel:
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