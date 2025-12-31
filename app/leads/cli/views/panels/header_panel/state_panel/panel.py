from textual.app import ComposeResult
from textual.containers import Vertical, Horizontal
from textual.widgets import Static


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

    def compose(self) -> ComposeResult:
        with Horizontal(classes="state-row"):
            yield Static("Active Forest:", classes="state-key")
            yield Static("Demo Forest (forest-001)", classes="state-value")
        with Horizontal(classes="state-row"):
            yield Static("Active Trail:", classes="state-key")
            yield Static("Demo Trail (trail-001)", classes="state-value")

