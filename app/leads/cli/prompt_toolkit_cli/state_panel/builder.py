from __future__ import annotations

from prompt_toolkit.layout import Window
from prompt_toolkit.layout.controls import FormattedTextControl

from ..cli_state import CliState


def __build_state_control(state: CliState) -> FormattedTextControl:
    def get_text():
        lines = []

        forest_key = "Active Forest: "
        forest_val = f"{state.current_forest_name} ({state.current_forest_id})"
        trail_key = "Active Trail:  "
        trail_val = f"{state.current_trail_name} ({state.current_trail_id})"

        lines.append(("class:title-key", forest_key))
        lines.append(("class:title-value", forest_val + "\n"))
        lines.append(("class:title-key", trail_key))
        lines.append(("class:title-value", trail_val))

        return lines

    return FormattedTextControl(get_text)


def build_state_panel(state: CliState) -> Window:
    return Window(
        height=2,
        content=__build_state_control(state),
        style="class:status-bar",
        always_hide_cursor=True,
    )
