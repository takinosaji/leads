from __future__ import annotations

from prompt_toolkit.layout import Window
from prompt_toolkit.layout.controls import FormattedTextControl

from ..cli_state import CliState


ASCII_TITLE = (
    " _        ______    _____     _____     _____\n"
    "|*|      |* ____|  / ___ \\   |* __ \\   / ____|\n"
    "|*|      |*|__    |*|   |*|  |*|  |*| |*(___\n"
    "|*|      |* __|   |*|___|*|  |*|  |*|  \\___ \\ \n"
    "|*|____  |*|____  |* ___|*|  |*|__|*|  ____)*|\n"
    "|______| |______| |_|   |_|  |_____/  |_____/\n"
)


def __build_title_control(state: CliState) -> FormattedTextControl:
    def get_text():
        return [("class:header-ascii", ASCII_TITLE)]

    return FormattedTextControl(get_text)


def build_title_panel(state: CliState) -> Window:
    return Window(
        height=6,
        content=__build_title_control(state),
        style="class:header-ascii",
        always_hide_cursor=True,
    )
