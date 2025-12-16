from prompt_toolkit.application import get_app
from prompt_toolkit.layout import Window
from prompt_toolkit.layout.controls import FormattedTextControl

from ..cli_state import CLI_TABS, CliState
from ..models import CliPanels

MENU_PANEL_WIDTH = 30


def __build_menu_control(state: CliState) -> FormattedTextControl:
    def get_fragments():
        app = get_app()

        fragments = []

        inner_width = MENU_PANEL_WIDTH - 2
        label_width = inner_width - len(" ") - len("▸ ") - len(" ")

        for i, item in enumerate(CLI_TABS):
            is_selected = i == state.menu_state.selected_index
            if is_selected:
                style = "class:menu-selected-active" if app.layout.has_focus(state.focusable_controls[CliPanels.MENU]) else "class:menu-selected-inactive"
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


def build_menu_panel(state: CliState) -> Window:
    menu_control = __build_menu_control(state)

    window = Window(
        content=menu_control,
        width=MENU_PANEL_WIDTH,
        style="class:menu",
        always_hide_cursor=True,
    )

    state.focusable_controls[CliPanels.MENU] = window

    return window
