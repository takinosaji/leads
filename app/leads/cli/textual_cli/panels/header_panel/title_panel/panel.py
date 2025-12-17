from textual.widgets import Static
from rich.align import Align
from rich.text import Text

ASCII_TITLE = (
    " _        ______    _____     _____     _____\n"
    "|*|      |* ____|  / ___ \\   |* __ \\   / ____|\n"
    "|*|      |*|__    |*|   |*|  |*|  |*| |*(___\n"
    "|*|      |* __|   |*|___|*|  |*|  |*|  \\___ \\ \n"
    "|*|____  |*|____  |* ___|*|  |*|__|*|  ____)*|\n"
    "|______| |______| |_|   |_|  |_____/  |_____/\n"
)


class TitlePanel(Static):
    DEFAULT_CSS = """
    TitlePanel {
        width: 1fr;
        content-align: center middle;
        color: #ffaf00;
    }
    """

    def __init__(self) -> None:
        # Render the multi-line ASCII as a single, non-wrapping block and
        # center it as a whole using Rich Align. This avoids per-line shifts.
        text = Text(ASCII_TITLE, no_wrap=True)
        super().__init__(Align.center(text, vertical="middle"))
