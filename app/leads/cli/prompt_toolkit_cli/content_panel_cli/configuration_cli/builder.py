from __future__ import annotations

from prompt_toolkit.layout import Window
from prompt_toolkit.layout.controls import FormattedTextControl
from prompt_toolkit.widgets import Frame

from ..cli_state import CliState


def build_configuration_panel(state: CliState) -> Frame:
    """Build the Configuration content panel.

    Placeholder for now; later this can be replaced with an editable table
    of configuration keys/values.
    """

    def get_text():
        return [
            ("class:content-title", "Configuration\n"),
            ("", "\n"),
            ("class:content-text", state.content_text + "\n"),
        ]

    control = FormattedTextControl(get_text)
    window = Window(content=control, style="class:content")
    return Frame(body=window, style="class:content")
