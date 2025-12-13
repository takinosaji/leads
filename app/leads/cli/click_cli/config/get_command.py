from typing import Callable

import click
from partial_injector.partial_container import Container
from returns.result import Result

from leads.cli.click_cli.types import CliCommandFactory


type ConfigGetCommandHandler = Callable[[str, str], Result[None]]


def __create_get_command(container: Container):
    handler = container.resolve(ConfigGetCommandHandler)

    cmd = click.Command(
        "get",
        params=[
            click.Option(["--debug"], is_flag=True),
            click.Argument(["filename"])
        ],
        callback=handler,
    )

    return cmd
create_get_command: CliCommandFactory = __create_get_command


def __handle_config_get_command():
    pass
handle_config_get_command: ConfigGetCommandHandler = __handle_config_get_command