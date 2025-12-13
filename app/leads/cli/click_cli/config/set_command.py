from typing import Callable

import click
from partial_injector.partial_container import Container
from returns.result import Result

from leads.cli.click_cli.types import CliCommandFactory


type ConfigSetCommandHandler = Callable[[str, str], Result[None]]


def __create_set_command(container: Container):
    handler = container.resolve(ConfigSetCommandHandler)

    cmd = click.Command(
        "set",
        params=[
            click.Option(["--debug"], is_flag=True),
            click.Argument(["filename"])
        ],
        callback=handler,
    )

    return cmd
create_set_command: CliCommandFactory = __create_set_command


def __handle_config_set_command():
    pass


handle_config_set_command: ConfigSetCommandHandler = __handle_config_set_command