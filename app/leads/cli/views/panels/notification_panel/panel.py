from textual.app import ComposeResult
from textual.containers import Container, Vertical, Horizontal
from textual.widgets import Static

from leads.cli.view_models.notification_view_model import NotificationViewModel


class NotificationPanel(Container):
    DEFAULT_CSS = """
    NotificationPanel {
        width: 1fr;
        height: 5;
        max-height: 5;
        padding: 0 0;
        background: #000000;
        color: #ffffff;
        border: heavy $primary;
        overflow-y: auto;
    }

    NotificationPanel.hidden {
        display: none;
    }

    NotificationPanel > .row {
        width: 1fr;
        height: auto;
        layout: horizontal;
        align-vertical: top;
    }

    NotificationPanel > .row > .label {
        width: 30;
        padding: 0 0;
        text-style: bold;
        background: #000000;
        color: #ffaf00;
    }

    NotificationPanel > .row > .notifications-container {
        width: 1fr;
        height: auto;
        layout: vertical;
        padding: 0 0;
        background: #000000;
        color: #ffffff;
    }

    NotificationPanel > .row > .notifications-container > .notification-row {
        width: 1fr;
        height: auto;
        layout: horizontal;
    }

    NotificationPanel > .row > .notifications-container > .notification-row > .type-label {
        width: 20;
        padding: 0 1;
        text-style: bold;
    }

    NotificationPanel > .row > .notifications-container > .notification-row > .notification-row > .notification-box {
        width: 1fr;
        height: auto;
        padding: 0 1;
        margin-top: 0;
        margin-bottom: 0;
        border: none;
        content-align: left top;
    }

    NotificationPanel > .row > .notifications-container > .notification-row.-error {
        background: #660000;
        color: #ffcccc;
    }

    NotificationPanel > .row > .notifications-container > .notification-row.-info {
        background: #003300;
        color: #aaffaa;
    }
    """

    def __init__(self, notification_view_model: NotificationViewModel, **kwargs) -> None:
        super().__init__(id="notifications", **kwargs)
        self.can_focus = False
        self.view_model = notification_view_model
        self._subscription = self.view_model.subscribe(self._on_view_model_changed)

    def on_focus(self, event) -> None:
        self.blur()
        event.stop()

    def _on_view_model_changed(self) -> None:
        if self.view_model.notifications:
            self.remove_class("hidden")
        else:
            self.add_class("hidden")

        self.refresh(recompose=True)

    def compose(self) -> ComposeResult:
        with Horizontal(classes="row"):
            yield Static("Notifications:", classes="label")
            with Vertical(classes="notifications-container"):
                for item in self.view_model.notifications:
                    row_class = "notification-row -error" if item.is_error else "notification-row -info"
                    with Horizontal(classes=row_class):
                        yield Static(f"[{item.timestamp.strftime("%H:%M:%S")}] " + ("ERROR:" if item.is_error else "INFO:"), classes="type-label")
                        yield Static(item.message, classes="notification-box")

    def on_mount(self) -> None:
        if not self.view_model.notifications:
            self.add_class("hidden")

    def on_unmount(self) -> None:
        self._subscription.dispose()
