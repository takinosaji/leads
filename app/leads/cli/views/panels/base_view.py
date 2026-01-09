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

    def __init__(self, title: str, **kwargs) -> None:
        super().__init__(classes="content-tab", **kwargs)
        self._title = title

    def compose(self) -> ComposeResult:
        yield Static(self._title)

    def handle_command(self, text: str) -> bool:
        return False

    def activate(self):
        return None

    def deactivate(self):
        return None
