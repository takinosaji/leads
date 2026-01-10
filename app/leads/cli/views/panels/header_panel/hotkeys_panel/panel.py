from textual.app import ComposeResult
from textual.containers import Horizontal, Vertical
from textual.widgets import Static

from leads.cli.view_models.hotkeys_view_model import HotkeysViewModel

class HotkeysPanel(Horizontal):
    DEFAULT_CSS = """
    HotkeysPanel {
        width: 60;
        padding: 0 1;
    }

    .hotkey-column {
        layout: vertical;
        margin-right: 2;
        width: 30;
    }

    .hotkey-row {
        layout: horizontal;
        margin: 0;
        padding: 0;
        height: 1;
    }

    .hotkey-symbol {
        width: 12;
        color: #ff4fcf;
        text-style: bold;
        padding-right: 1;
    }

    .hotkey-definition {
        color: #888888;
        padding-left: 0;
    }
    """

    def __init__(self,
                 hotkeys_view_model: HotkeysViewModel,
                 **kwargs):
        super().__init__(**kwargs)
        self.can_focus = False
        self.view_model = hotkeys_view_model
        self._subscription = self.view_model.subscribe(self._on_view_model_changed)

    def _on_view_model_changed(self) -> None:
        self.refresh(recompose=True)

    def compose(self) -> ComposeResult:
        for column in self.view_model.hotkeys:
            with Vertical(classes="hotkey-column"):
                for hotkey in column:
                    with Horizontal(classes="hotkey-row"):
                        yield Static(hotkey.symbol, classes="hotkey-symbol")
                        yield Static(hotkey.definition, classes="hotkey-definition")

    def on_unmount(self) -> None:
        self._subscription.dispose()
