from textual.app import ComposeResult
from textual.containers import Container
from textual.widgets import Static


class BaseView(Container):
    DEFAULT_CSS = """
    BaseView {
        width: 1fr;
        height: 1fr;
        content-align: center middle;
    }
    """

    def __init__(self, title: str, *, id: str | None = None) -> None:
        super().__init__(id=id, classes="content-tab")
        self._title = title

    def compose(self) -> ComposeResult:
        yield Static(self._title)

    def handle_command(self, text: str) -> bool:
        return False
