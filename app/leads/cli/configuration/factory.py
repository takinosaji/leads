import os
import yaml
from pathlib import Path
from typing import Callable
from returns.result import Result, safe

from .configuration_logging import get_default_logger, LogLevel
from .models import CliConfiguration, RuntimeConfiguration, ContextConfiguration, CliConfigurationCache
from leads.secondary_adapters.sqlite_adapter.configuration import SQLiteStorageConfiguration
from leads.secondary_adapters.mongodb_adapter.configuration import MongoDbStorageConfiguration

type CliConfigurationLoader = Callable[[], Result[CliConfiguration]]
type CliConfigurationSaver = Callable[[CliConfiguration], Result]


@safe
def __load_n_cache_cli_configuration(dep_configuration_cache: CliConfigurationCache,
                                     dep_save_cli_configuration: CliConfigurationSaver) -> CliConfiguration:
    logger = get_default_logger()

    file_path = __get_config_file_path()
    if not os.path.exists(file_path):
        dep_save_cli_configuration(
            CliConfiguration(
                context_configuration=ContextConfiguration(
                    active_forest=None,
                ),
                runtime_configuration=RuntimeConfiguration(
                    min_log_level=LogLevel.INFO,
                ),
                sqlite_storage_configuration=SQLiteStorageConfiguration(
                    connection_string=f"sqlite:////{__get_leads_folder_path()}/leads.db",
                ),
                mongodb_storage_configuration=MongoDbStorageConfiguration(
                    connection_string=None,
                    database_name=None,
                    timeout_ms=2000,
                ),
            )
        )

    try:
        with open(file_path, "r", encoding="utf-8") as fh:
            data = yaml.safe_load(fh) or {}
    except Exception as exc:
        logger.error("Failed to read configuration file", exc_info=exc)
        raise

    try:
        configuration = CliConfiguration(**data)
        dep_configuration_cache.configuration = configuration
    except Exception as exc:
        logger.error("Invalid configuration contents", exc_info=exc)
        raise

    return configuration
load_n_cache_cli_configuration: CliConfigurationLoader = __load_n_cache_cli_configuration


def __get_leads_folder_path() -> Path:
    home = Path.home()
    return home / ".leads"


def __get_config_file_path() -> Path:
    return __get_leads_folder_path() / "config.yaml"


@safe
def __save_cli_configuration(dep_configuration_cache: CliConfigurationCache,
                             configuration: CliConfiguration):
    config_path = __get_config_file_path()
    config_path.parent.mkdir(parents=True, exist_ok=True)
    configuration_text = yaml.safe_dump(configuration.model_dump())
    config_path.write_text(configuration_text, encoding="utf-8")

    dep_configuration_cache.configuration = configuration

    return None
save_cli_configuration: CliConfigurationSaver = __save_cli_configuration