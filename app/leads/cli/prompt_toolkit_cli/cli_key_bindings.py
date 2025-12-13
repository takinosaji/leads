from __future__ import annotations

from prompt_toolkit.key_binding import KeyBindings
from prompt_toolkit.keys import Keys

from .cli_state import CliState, CLI_TABS
from .content_panel.configuration.key_bindings import configuration_handle_up_key, configuration_handle_down_key, \
    configuration_handle_any_key
from .models import CliMode, CliTab


def build_prompt_toolkit_key_bindings(state: CliState) -> KeyBindings:
    kb = KeyBindings()

    @kb.add("tab")
    def __(event):
        if state.mode is not CliMode.NAVIGATION:
            return
        state.focus_index = 1 - state.focus_index
        event.app.invalidate()

    @kb.add("up")
    def __(event):
        if state.mode is not CliMode.NAVIGATION:
            return
        if state.focus_index == 0:
            state.selected_index = (state.selected_index - 1) % len(CLI_TABS)
            state.clear_notification()
        else:
            match state.selected_item:
                case CliTab.CONFIGURATION:
                    configuration_handle_up_key(state, event)
                case _:
                    pass
        event.app.invalidate()

    @kb.add("down")
    def __(event):
        if state.mode is not CliMode.NAVIGATION:
            return
        if state.focus_index == 0:
            state.selected_index = (state.selected_index + 1) % len(CLI_TABS)
            state.clear_notification()
        else:
            match state.selected_item:
                case CliTab.CONFIGURATION:
                    configuration_handle_down_key(state, event)
                case _:
                    pass
        event.app.invalidate()

    @kb.add(":")
    def __(event):
        if state.mode is CliMode.NAVIGATION:
            state.mode = CliMode.COMMAND
            state.command_buffer = ":"
            event.app.invalidate()

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
