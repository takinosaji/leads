from textual.app import ComposeResult
from textual.containers import Container as TContainer

from leads.cli.textual_cli.panels.header_panel.state_panel.panel import StatePanel
from leads.cli.textual_cli.panels.header_panel.title_panel.panel import TitlePanel


class HeaderPanel(TContainer):
    DEFAULT_CSS = """
    HeaderPanel {
        height: 6;
        dock: top;
        layout: horizontal;
        background: #202020;
    }
    """

    def compose(self) -> ComposeResult:
        yield StatePanel()
        yield TitlePanel()

