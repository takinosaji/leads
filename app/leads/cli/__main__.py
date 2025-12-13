from leads.cli.dependency_injection.container import get_container
from leads.cli.prompt_toolkit_cli.cli_builder import build_injected_cli

if __name__ == '__main__':
    container = get_container()
    injected_cli = build_injected_cli(container)
    injected_cli()