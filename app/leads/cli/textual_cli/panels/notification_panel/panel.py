from __future__ import annotations

from textual.app import ComposeResult
from textual.containers import Container, Vertical
from textual.widgets import Static


class NotificationPanel(Container):
    """Notification panel showing multiple notifications, each in its own box.

    The panel is intended to be non-focusable and conditionally shown/hidden
    via its public methods based on the internal notifications state.
    """

    DEFAULT_CSS = """
    NotificationPanel {
        width: 1fr;
        height: auto;
        dock: bottom;
        padding: 0 1;
        background: #202020;
        color: #ffffff;
    }

    NotificationPanel.hidden {
        display: none;
    }

    NotificationPanel > .notifications-container {
        width: 1fr;
        height: auto;
        layout: vertical;
        padding: 0 0;
    }

    NotificationPanel > .notifications-container > .notification-box {
        width: 1fr;
        height: auto;
        padding: 0 1;
        margin-top: 0;
        margin-bottom: 0;
        border: round #268bd2;
        background: #202020;
        color: #ffffff;
        content-align: left top;
    }
    """

    def __init__(self, **kwargs) -> None:
        super().__init__(**kwargs)
        self.can_focus = False
        self._notifications: list[str] = []

    def compose(self) -> ComposeResult:
        with Vertical(classes="notifications-container"):
            for message in self._notifications:
                yield Static(message, classes="notification-box")

    def on_mount(self) -> None:
        if not self._notifications:
            self.add_class("hidden")
