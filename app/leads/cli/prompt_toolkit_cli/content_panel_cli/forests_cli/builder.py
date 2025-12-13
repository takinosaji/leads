from __future__ import annotations

from prompt_toolkit.layout import Window
from prompt_toolkit.layout.controls import FormattedTextControl
from prompt_toolkit.widgets import Frame

from ..cli_state import CliState


def build_forests_panel(state: CliState) -> Frame:
    """Build the Forests content panel.

    Placeholder that can be evolved into a rich forests view.
    """

    def get_text():
        lines = [
            ("class:content-title", "Forests\n"),
            ("", "\n"),
            ("class:content-text", state.content_text + "\n"),
        ]
        return lines

    control = FormattedTextControl(get_text)
    window = Window(content=control, style="class:content")
    return Frame(body=window, style="class:content")

