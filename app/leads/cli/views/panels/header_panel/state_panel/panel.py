from textual.app import ComposeResult
from textual.containers import Vertical, Horizontal
from textual.widgets import Static

from leads.cli.view_models.state_panel_view_model import StatePanelViewModel


class StatePanel(Vertical):
    DEFAULT_CSS = """
    StatePanel {
        width: 42;
        padding: 0 1;
    }

    StatePanel > .state-row {
        layout: horizontal;
        margin: 0;
        padding: 0;
        height: 1;
    }

    StatePanel > .state-row > .state-key {
        width: 14;
        color: #ffaf00;
        text-style: bold;
    }

    StatePanel > .state-row > .state-value {
        color: #ffffff;
    }
    """

    def __init__(self, view_model: StatePanelViewModel, **kwargs):
        super().__init__(**kwargs)
        self.can_focus = False
        self.view_model = view_model
        self._subscription = self.view_model.subscribe(self._on_view_model_changed)

    def _on_view_model_changed(self) -> None:
        self.refresh(recompose=True)

    def compose(self) -> ComposeResult:
        active_forest = self.view_model.active_forest or "-"
        active_trail = self.view_model.active_trail or "-"

        with Horizontal(classes="state-row"):
            yield Static("Active Forest:", classes="state-key")
            yield Static(active_forest, classes="state-value")
        with Horizontal(classes="state-row"):
            yield Static("Active Trail:", classes="state-key")
            yield Static(active_trail, classes="state-value")

    def on_unmount(self) -> None:
        if self._subscription:
            self._subscription.dispose()
            self._subscription = None
