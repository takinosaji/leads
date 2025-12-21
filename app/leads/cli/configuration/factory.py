import os
from marshal import loads

import yaml
from pathlib import Path
from typing import Callable

from returns.result import Result, safe

from .logging import get_default_logger, LogLevel

from leads.cli.configuration.models import CliConfiguration, RuntimeConfiguration, ContextConfiguration


type CliConfigurationLoader = Callable[[], Result[CliConfiguration]]
type CliConfigurationSaver = Callable[[CliConfiguration], Result]


@safe
def __create_cli_configuration(dep_save_cli_configuration: CliConfigurationSaver) -> CliConfiguration:
    logger = get_default_logger()

    file_path = __get_config_file_path()
    if not os.path.exists(file_path):
        dep_save_cli_configuration(CliConfiguration(context_configuration=ContextConfiguration(
                                                        active_forest=None
                                                    ),
                                                    runtime_configuration=RuntimeConfiguration(
                                                        min_log_level=LogLevel.INFO
                                                    )
        ))

    try:
        with open(file_path, "r", encoding="utf-8") as fh:
            data = yaml.safe_load(fh) or {}
    except Exception as exc:
        logger.error("Failed to read configuration file", exc_info=exc)
        raise

    try:
        configuration = CliConfiguration(**data)
    except Exception as exc:
        logger.error("Invalid configuration contents", exc_info=exc)
        raise

    return configuration
load_cli_configuration: CliConfigurationLoader = __create_cli_configuration


def __get_config_file_path() -> Path:
    home = Path.home()
    return home / ".leads" / "config.yaml"


@safe
def __save_cli_configuration(configuration: CliConfiguration):
    config_path = __get_config_file_path()
    config_path.parent.mkdir(parents=True, exist_ok=True)
    configuration_text = yaml.safe_dump(configuration.model_dump())
    config_path.write_text(configuration_text, encoding="utf-8")

    return None
save_cli_configuration: CliConfigurationSaver = __save_cli_configuration