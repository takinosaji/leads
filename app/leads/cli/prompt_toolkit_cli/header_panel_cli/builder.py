from __future__ import annotations

from prompt_toolkit.layout import Window
from prompt_toolkit.layout.controls import FormattedTextControl
from prompt_toolkit.widgets import Frame

from .cli_state import CliState


def _get_terminal_width_or_title(title: str) -> int:
    try:
        from prompt_toolkit.application.current import get_app_or_none
    except Exception:
        return len(title)

    app = get_app_or_none()
    if app is None:
        return len(title)

    try:
        return app.output.get_size().columns
    except Exception:
        return len(title)


def _build_header_control(state: CliState) -> FormattedTextControl:
    def get_text():
        title = " Leads "
        width = _get_terminal_width_or_title(title)

        total_padding = max(width - len(title), 0)
        left_padding = total_padding // 2
        right_padding = total_padding - left_padding

        line = " " * left_padding + title + " " * right_padding
        return [("class:header-ascii", line)]

    return FormattedTextControl(get_text)


def build(state: CliState) -> Frame:
    header_window = Window(
        height=1,
        content=_build_header_control(state),
        style="class:header-ascii",
        always_hide_cursor=True,
    )

    framed_header = Frame(
        body=header_window,
        style="class:header-ascii",
    )

    return framed_header
