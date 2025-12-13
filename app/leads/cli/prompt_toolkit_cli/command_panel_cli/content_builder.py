from __future__ import annotations

from prompt_toolkit.layout import Window
from prompt_toolkit.layout.controls import FormattedTextControl

from .cli_state import CliState


def _build_command_control(state: CliState) -> FormattedTextControl:
    def get_text():
        return [("class:status-bar", state.command_line_text)]

    return FormattedTextControl(get_text)


def build(state: CliState) -> Window:
    return Window(
        height=1,
        content=_build_command_control(state),
        style="class:status-bar",
    )
