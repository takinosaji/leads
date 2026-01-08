from textual.app import ComposeResult
from textual.containers import Vertical, Horizontal
from textual.screen import ModalScreen
from textual.widgets import Label, Button, Input, Checkbox, TextArea

from leads.cli.view_models.forests_view_model import ForestDto, ForestsViewModel
from leads.cli.view_models.notification_view_model import NotificationViewModel


class ForestCreationUpdateModal(ModalScreen):
    DEFAULT_CSS = """
    ForestCreationUpdateModal {
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
                 notification_view_model: NotificationViewModel,
                 title: str,
                 button_text: str,
                 archived_locked: bool = False) -> None:
        super().__init__()

        self._view_model = view_model
        self._notification_view_model = notification_view_model
        self._archived_locked = archived_locked
        self._title = title
        self._button_text = button_text

    def compose(self) -> ComposeResult:
        with Vertical(id="modal"):
            yield Label(self._title, id="title")

            with Horizontal(classes="row"):
                yield Label("Name:", classes="label")
                yield Input(placeholder="Enter name", id="name", classes="input")

            with Horizontal(classes="row"):
                yield Label("Description:", classes="label")
                yield TextArea(
                    text="",
                    id="description",
                    classes="input",
                )

            if not self._archived_locked:
                with Horizontal(classes="row"):
                    yield Label("Archived:", classes="label")
                    yield Checkbox(id="archived")

            with Horizontal(id="buttons"):
                yield Button(self._button_text, id="ok", variant="primary")

    def on_button_pressed(self, event):
        if event.button.id == "ok":
            name = self.query_one("#name", Input).value
            description = self.query_one("#description", TextArea).text
            archived = not self._archived_locked and self.query_one("#archived", Checkbox).value
            # self._view_model.create_forest(ForestDto(name=name,
            #                                          description=description,
            #                                          is_archived=archived))
            # self.dismiss()
            self._notification_view_model.add_notification("hi", is_error=True)
        else:
            self.dismiss()

    def on_key(self, event):
        match event.key:
            case "escape":
                self.dismiss(None)
                event.stop()
