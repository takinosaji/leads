from __future__ import annotations

from prompt_toolkit.key_binding import KeyBindings
from prompt_toolkit.keys import Keys

from .cli_state import CliState, CLI_TABS
from .models import CliMode


def build_prompt_toolkit_key_bindings(state: CliState) -> KeyBindings:
    kb = KeyBindings()

    @kb.add("up")
    def _(event):
        if state.mode is not CliMode.NAVIGATION:
            return
        state.selected_index = (state.selected_index - 1) % len(CLI_TABS)
        event.app.invalidate()

    @kb.add("down")
    def _(event):
        if state.mode is not CliMode.NAVIGATION:
            return
        state.selected_index = (state.selected_index + 1) % len(CLI_TABS)
        event.app.invalidate()

    @kb.add(":")
    def _(event):
        if state.mode is CliMode.NAVIGATION:
            state.mode = CliMode.EDITING
            state.command_buffer = ":"
            event.app.invalidate()

    @kb.add(Keys.Any)
    def _(event):
        if state.mode is not CliMode.EDITING:
            return
        data = event.data
        if data == "\r":
            cmd = state.command_buffer.strip()
            if cmd == ":q":
                event.app.exit()
            state.mode = CliMode.NAVIGATION
            state.command_buffer = ""
            event.app.invalidate()
        elif data in ("\x1b",):
            state.mode = CliMode.NAVIGATION
            state.command_buffer = ""
            event.app.invalidate()
        elif data in ("\x08", "\x7f"):
            state.command_buffer = state.command_buffer[:-1]
            event.app.invalidate()
        else:
            state.command_buffer += data
            event.app.invalidate()

    @kb.add("c-c")
    def _(event):
        if state.mode is CliMode.NAVIGATION:
            event.app.exit()

    return kb
