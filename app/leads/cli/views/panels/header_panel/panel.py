from textual.app import ComposeResult
from textual.containers import Container as TContainer

from leads.cli.views.panels.header_panel.state_panel.panel import StatePanel
from leads.cli.views.panels.header_panel.title_panel.panel import TitlePanel
from leads.cli.views.panels.header_panel.hotkeys_panel.panel import HotkeysPanel


class HeaderPanel(TContainer):
    DEFAULT_CSS = """
    HeaderPanel {
        height: 6;
        dock: top;
        layout: horizontal;
        background: #202020;
    }
    """

    def __init__(self,
                 hotkeys_view_model,
                 **kwargs):
        super().__init__(id="header", **kwargs)

        self._state_panel = StatePanel()
        self._title_panel = TitlePanel()
        self.hotkeys_panel = HotkeysPanel(hotkeys_view_model)

    def compose(self) -> ComposeResult:
        yield self._state_panel
        yield self.hotkeys_panel
        yield self._title_panel