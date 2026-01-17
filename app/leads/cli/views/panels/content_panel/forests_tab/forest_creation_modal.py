from returns.pointfree import bind, lash
from returns.result import safe
from returns.pipeline import flow
from textual.app import ComposeResult
from textual.containers import Vertical, Horizontal
from textual.screen import ModalScreen
from textual.widgets import Label, Button, Input, Checkbox, TextArea

from leads.application_core.secondary_ports.forests import NewForestDto, Forest
from leads.cli.view_models.forests_view_model import ForestsViewModel
from leads.cli.view_models.notification_view_model import NotificationViewModel


class ForestCreationModal(ModalScreen):
    DEFAULT_CSS = """
    ForestCreationModal {
        align: center middle;
    }

    #modal {
        width: 80;
        height: 20;
        padding: 1 2;
        background: $panel;
    }

    #title {
        width: 100%;
        text-align: center;
        text-style: bold;
        color: #ffd787;
        margin-bottom: 1;
    }

    .row {
        align: left middle;
        margin-bottom: 1;
    }

    .label {
        width: 14;
    }

    .input {
        width: 1fr;
    }

    TextArea {
        height: 4;
    }

    #buttons {
        margin-top: 1;
        align: right middle;
    }
    """

    def __init__(self,
                 view_model: ForestsViewModel,
                 notification_view_model: NotificationViewModel) -> None:
        super().__init__()

        self._view_model = view_model
        self._notification_view_model = notification_view_model

    def compose(self) -> ComposeResult:
        with Vertical(id="modal"):
            yield Label("Create Forest", id="title")

            with Horizontal(classes="row"):
                yield Label("Name:", classes="label")
                yield Input(placeholder="Enter name", id="name", classes="input", select_on_focus=False)

            with Horizontal(classes="row"):
                yield Label("Description:", classes="label")
                yield TextArea(
                    text="",
                    id="description",
                    classes="input",
                )

            with Horizontal(id="buttons"):
                yield Button("Create", id="ok", variant="primary")

    def on_button_pressed(self, event):
        if event.button.id == "ok":
            name = self.query_one("#name", Input).value
            description = self.query_one("#description", TextArea).text

            @safe
            def success(forest: Forest):
                self._notification_view_model.add_notification(f"Forest {forest.name} created successfully.", is_error=False)
                self._view_model.invalidate_data()
                self.dismiss()

            @safe
            def fail(error: Exception):
                self._notification_view_model.add_notification(str(error), is_error=True)

            self._notification_view_model.clear_notifications()
            flow(NewForestDto(name=name, description=description),
                 self._view_model.create_forest,
                 bind(success),
                 lash(fail)).unwrap()
        else:
            self.dismiss()

    def on_key(self, event):
        match event.key:
            case "escape":
                self.dismiss(None)
                event.stop()
