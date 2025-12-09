import click

from leads.cli.dependency_injection.container import get_container
from leads.cli.main import main


@click.command()
def cli_handler():
    container = get_container()
    injected_main = container.resolve(main)
    injected_main()

if __name__ == '__main__':
    cli_handler()