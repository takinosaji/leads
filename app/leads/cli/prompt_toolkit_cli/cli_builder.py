from partial_injector.partial_container import Container
from prompt_toolkit import Application
from prompt_toolkit.layout import HSplit, VSplit, Layout
from prompt_toolkit.layout.containers import DynamicContainer, Window

from .cli_key_bindings import build_prompt_toolkit_key_bindings
from .cli_state import CliState
from .menu_panel.builder import build_menu_panel
from .content_panel.builder import build_content_panel
from .state_panel.builder import build_state_panel
from .title_panel.builder import build_title_panel
from .command_panel.builder import build_command_panel
from .notification_panel.builder import build_notification_panel
from .cli_styles import build_style
from .models import CliMode, CliPanels


def build_injected_cli(container: Container):
    state = CliState()

    title_panel = build_title_panel(state)
    menu_panel = build_menu_panel(state)

    content_container = DynamicContainer(lambda: build_content_panel(state, container))
    #state.focusable_controls[CliPanels.CONTENT_CONTAINER] = content_container

    state_panel = build_state_panel(state)

    def _notification_container_factory():
        if state.notification_message:
            return build_notification_panel(state)
        return Window(height=0)

    notification_container = DynamicContainer(_notification_container_factory)

    def _command_container_factory():
        if state.mode is CliMode.COMMAND:
            return build_command_panel(state)
        return Window(height=0)

    command_container = DynamicContainer(_command_container_factory)

    root_container = HSplit(
        [
            VSplit(
                [
                    state_panel,
                    title_panel
                ],
            ),
            VSplit(
                [
                    menu_panel,
                    content_container,
                ]
            ),
            notification_container,
            command_container,
        ]
    )

    layout = Layout(root_container)
    state.focus_menu_panel(layout)

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
