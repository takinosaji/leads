import click
from partial_injector.partial_container import Container

from leads.cli.click_cli.config.config_builder import build_config_command


def build_injected_cli(container: Container):
    cli = click.Group()

    cli.add_command(build_config_command(container))

    return cli