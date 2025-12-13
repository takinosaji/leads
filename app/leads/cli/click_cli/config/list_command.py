from typing import Callable
import click
from partial_injector.partial_container import Container
from returns.result import Result

from leads.cli.click_cli.types import CliCommandFactory


type ConfigListCommandHandler = Callable[[], Result[None]]


def __create_config_list_command(container: Container):
    handler = container.resolve(ConfigListCommandHandler)

    cmd = click.Command(
        "list",
        params=[],
        help="The list command retrieves all config keys and values",
        callback=handler,
    )

    return cmd
create_list_command: CliCommandFactory = __create_config_list_command

def __handle_config_list_command():
    pass
handle_config_list_command: ConfigListCommandHandler = __handle_config_list_command