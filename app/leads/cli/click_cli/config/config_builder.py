import click
from partial_injector.partial_container import Container

from leads.cli.click_cli.config.get_command import create_get_command
from leads.cli.click_cli.config.list_command import create_list_command
from leads.cli.click_cli.config.set_command import create_set_command


def build_config_command(container: Container):
    config = click.Group("config")

    config.add_command(create_list_command(container))
    config.add_command(create_set_command(container))
    config.add_command(create_get_command(container))

    return config