from prompt_toolkit.key_binding import KeyBindings
from prompt_toolkit.keys import Keys

from .cli_state import CliState, CLI_TABS
from .content_panel.configuration.key_bindings import configuration_handle_up_key, configuration_handle_down_key, \
    configuration_handle_any_key
from .menu_panel.key_bindings import menu_handle_up_key
from .models import CliMode, CliTab, CliPanels


def build_prompt_toolkit_key_bindings(state: CliState) -> KeyBindings:
    kb = KeyBindings()

    @kb.add("tab")
    def __(event):
        layout = event.app.layout
        match state.mode:
            case CliMode.NAVIGATION:
                state.focus_next_control(layout)
            case _:
                pass


    @kb.add("up")
    def __(event):
        match state.mode:
            case CliMode.NAVIGATION:
                match state.focused_control:
                    case _ if state.focused_control is state.focusable_controls[CliPanels.MENU]:
                        menu_handle_up_key(state, event)
                    case CliTab.CONFIGURATION:
                        configuration_handle_up_key(state, event)
                    case _:
                        pass
            case _:
                pass


    @kb.add("down")
    def __(event):
        if state.mode is not CliMode.NAVIGATION:
            return
        if state.focus_index == 0:
            state.menu_state.selected_index = (state.menu_state.selected_index + 1) % len(CLI_TABS)
            state.clear_notification()
        else:
            match state.menu_state.selected_item:
                case CliTab.CONFIGURATION:
                    configuration_handle_down_key(state, event)
                case _:
                    pass
        event.app.invalidate()

    @kb.add(":")
    def __(event):
        match state.mode:
            case CliMode.NAVIGATION:
                state.mode = CliMode.COMMAND
                state.command_buffer = ":"
                event.app.invalidate()
                state.focus_command_panel(event.app.layout)

    @kb.add(Keys.Any)
    def __(event):
        match state.mode:
            case CliMode.COMMAND:
                data = event.data

                if data == "\r":
                    cmd = state.command_buffer.strip()
                    match cmd:
                        case ":q":
                            event.app.exit()
                        case ":i":
                            state.mode = CliMode.EDITING
                            event.app.invalidate()
                        case _:
                            state.mode = CliMode.NAVIGATION
                            event.app.invalidate()
                elif data in ("\x1b",):
                    state.mode = CliMode.NAVIGATION
                    state.command_buffer = ""
                    event.app.invalidate()
                elif data in ("\x08", "\x7f"):
                    state.command_buffer = state.command_buffer[:-1]
                    if not state.command_buffer:
                        state.mode = CliMode.NAVIGATION
                    event.app.invalidate()
                else:
                    state.command_buffer += data
                    event.app.invalidate()
            case CliMode.EDITING:
                match state.selected_item:
                    case CliTab.CONFIGURATION:
                        configuration_handle_any_key(state, event)
                    case _:
                        pass

    @kb.add("c-c")
    def __(event):
        if state.mode is CliMode.NAVIGATION:
            event.app.exit()

    return kb
