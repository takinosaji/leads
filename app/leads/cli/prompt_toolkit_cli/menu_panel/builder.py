from __future__ import annotations

from typing import Callable

from prompt_toolkit.layout import Window
from prompt_toolkit.layout.controls import FormattedTextControl

from ..cli_state import CLI_TABS, CliState


MENU_PANEL_WIDTH = 30


def __build_menu_control(state: CliState, app_getter: Callable[[], object]) -> FormattedTextControl:
    def get_fragments():
        fragments = []
        # Inner width for the whole box content (without the border chars).
        inner_width = MENU_PANEL_WIDTH - 2
        # Reserve space inside for one leading space, prefix, label, and trailing space.
        label_width = inner_width - len(" ") - len("▸ ") - len(" ")
        for i, item in enumerate(CLI_TABS):
            is_selected = i == state.selected_index
            if is_selected:
                style = "class:menu-selected-active" if state.focus_index == 0 else "class:menu-selected-inactive"
            else:
                style = "class:menu-item"
            prefix = "▸ " if is_selected else "  "

            # Left-align the label text within the label_width.
            label = item.ljust(label_width)

            inner = f" {prefix}{label} "
            top = "┌" + "─" * inner_width + "┐\n"
            middle = "│" + inner + "│\n"
            bottom = "└" + "─" * inner_width + "┘\n"

            box = top + middle + bottom

            fragments.append((style, box))
        return fragments

    return FormattedTextControl(get_fragments)


def build_menu_panel(state: CliState, app_getter: Callable[[], object] | None = None) -> Window:
    if app_getter is None:
        app_getter = lambda: None

    menu_control = __build_menu_control(state, app_getter)

    menu_window = Window(
        content=menu_control,
        width=MENU_PANEL_WIDTH,
        style="class:menu",
        always_hide_cursor=True,
    )

    return menu_window
