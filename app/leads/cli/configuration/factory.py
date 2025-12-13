import os
import yaml
from pathlib import Path
from typing import Callable
from .logging import get_default_logger

from leads.cli.configuration.models import CliConfiguration, RuntimeConfiguration, ContextConfiguration


type ConfigurationFactory = Callable[[], CliConfiguration]


def __create_configuration() -> CliConfiguration:
    logger = get_default_logger()

    file_path = __get_config_file_path()
    if not os.path.exists(file_path):
        __create_default_configuration(file_path)

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
create_configuration: ConfigurationFactory = __create_configuration


def __get_config_file_path() -> Path:
    home = Path.home()
    return home / ".leads" / "config.yaml"


def __create_default_configuration(filepath: Path) -> Path:
    config_path = Path(filepath)
    config_path.parent.mkdir(parents=True, exist_ok=True)

    default_configuration = CliConfiguration(
        context_configuration=ContextConfiguration(
            active_forest=None
        ),
        runtime_configuration=RuntimeConfiguration(
            min_log_level='INFO'
        )
    )

    configuration_text = yaml.safe_dump(default_configuration.model_dump())

    config_path.write_text(configuration_text, encoding="utf-8")

    return config_path