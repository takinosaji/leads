from partial_injector.partial_container import Container
from prompt_toolkit import Application
from prompt_toolkit.layout import HSplit, VSplit, Layout
from prompt_toolkit.layout.containers import DynamicContainer

from .cli_key_bindings import build_prompt_toolkit_key_bindings

from .cli_state import CliState
from .menu_panel_builder import build as build_menu_panel
from .content_panel_builder import build as build_content_panel
from .state_panel_builder import build as build_state_panel
from .header_panel_builder import build as build_header_panel
from .styles import build_style
from .command_panel_builder import build as build_command_panel


def build_injected_cli(container: Container):
    state = CliState()

    header_panel = build_header_panel(state)
    menu_panel = build_menu_panel(state)

    content_container = DynamicContainer(lambda: build_content_panel(state))

    command_panel = build_command_panel(state)
    state_panel = build_state_panel(state)

    root_container = HSplit(
        [
            header_panel,
            VSplit(
                [
                    menu_panel,
                    content_container,
                ]
            ),
            state_panel,
            command_panel,
        ]
    )

    layout = Layout(root_container)

    style = build_style()

    kb = build_prompt_toolkit_key_bindings(state)

    application = Application(
        layout=layout,
        key_bindings=kb,
        full_screen=True,
        mouse_support=True,
        style=style,
    )

    return application.run
