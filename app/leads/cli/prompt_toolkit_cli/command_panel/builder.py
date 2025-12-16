from prompt_toolkit.layout import Window
from prompt_toolkit.layout.controls import FormattedTextControl

from ..cli_state import CliState
from ..models import CliPanels


def __build_command_control(state: CliState) -> FormattedTextControl:
    def get_text():
        label = "~> "
        text = state.command_line_text or ""
        return [
            ("class:command-bar", label),
            ("class:command-bar", text),
        ]

    return FormattedTextControl(get_text)


def build_command_panel(state: CliState) -> Window:
    window = Window(
        height=1,
        content=__build_command_control(state),
        style="class:command-bar",
    )

    state.focusable_controls[CliPanels.COMMAND] = window
    return window