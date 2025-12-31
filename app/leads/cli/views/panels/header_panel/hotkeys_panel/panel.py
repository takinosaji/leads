from textual.app import ComposeResult
from textual.containers import Horizontal, Vertical
from textual.widgets import Static

from leads.cli.view_models.hotkeys_view_model import HotkeysViewModel

class HotkeysPanel(Vertical):
    DEFAULT_CSS = """
    HotkeysPanel {
        width: 38;
        padding: 0 1;
    }

    HotkeysPanel > .hotkey-row {
        layout: horizontal;
        margin: 0;
        padding: 0;
        height: 1;
    }

    HotkeysPanel > .hotkey-row > .hotkey-symbol {
        width: 6;
        color: #ff4fcf;
        text-style: bold;
        padding-right: 1;
    }

    HotkeysPanel > .hotkey-row > .hotkey-definition {
        color: #888888;
        padding-left: 0;
    }
    """

    def __init__(self, hotkeys_view_model: HotkeysViewModel, **kwargs):
        super().__init__(**kwargs)
        self.can_focus = False
        self.view_model = hotkeys_view_model
        self._subscription = self.view_model.subscribe(self._on_view_model_changed)

    def _on_view_model_changed(self) -> None:
        self.refresh(recompose=True)

    def compose(self) -> ComposeResult:
        for hotkey in self.view_model.hotkeys:
            with Horizontal(classes="hotkey-row"):
                yield Static(hotkey.symbol, classes="hotkey-symbol")
                yield Static(hotkey.definition, classes="hotkey-definition")

    def on_unmount(self) -> None:
        self._subscription.dispose()
