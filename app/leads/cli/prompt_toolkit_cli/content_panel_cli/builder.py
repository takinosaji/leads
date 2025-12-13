from __future__ import annotations

from prompt_toolkit.widgets import Frame

from .cli_state import CliState
from .configuration_cli.content_builder import build_configuration_panel
from .forests_cli.content_builder import build_forests_panel
from .trails_cli.content_builder import build_trails_panel


def build(state: CliState) -> Frame:
    """Build and return the content panel for the TUI.

    The concrete content is selected based on the current menu selection
    in CliState (Configuration, Forests, Trails).
    """
    if state.selected_item == "Configuration":
        return build_configuration_panel(state)
    if state.selected_item == "Forests":
        return build_forests_panel(state)
    if state.selected_item == "Trails":
        return build_trails_panel(state)

    return build_configuration_panel(state)
