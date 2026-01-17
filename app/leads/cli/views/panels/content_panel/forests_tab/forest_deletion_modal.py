from textual.app import ComposeResult
from textual.containers import Vertical, Horizontal
from textual.screen import ModalScreen
from textual.widgets import Label, Button

from leads.cli.view_models.forests_view_model import ForestsViewModel
from leads.cli.view_models.notification_view_model import NotificationViewModel

class ForestDeletionModal(ModalScreen):
    DEFAULT_CSS = """
    ForestDeletionModal {
        align: center middle;
    }

    #modal {
        width: 60;
        height: 15;
        padding: 2 3;
        background: $panel;
    }

    #title {
        width: 100%;
        text-align: center;
        text-style: bold;
        color: #ffd787;
        margin-bottom: 1;
    }

    #warning {
        color: red;
        text-style: bold;
        text-align: center;
        margin-bottom: 1;
    }

    #question {
        text-align: center;
        margin-bottom: 2;
    }

    #buttons {
        align: center middle;
        margin-top: 2;
    }

    Button {
        min-width: 10;
        margin: 0 2;
    }
    """

    def __init__(self, view_model: ForestsViewModel, notification_view_model: NotificationViewModel) -> None:
        super().__init__()
        self._view_model = view_model
        self._notification_view_model = notification_view_model
        self._forest_name = view_model.selected_forest.name
        self._forest_id = view_model.selected_forest.id

    def compose(self) -> ComposeResult:
        with Vertical(id="modal"):
            yield Label("Delete Forest", id="title")
            yield Label(f"Are you sure, you want to delete Forest '{self._forest_name}'?", id="question")
            yield Label("This action is irreversible!", id="warning")
            with Horizontal(id="buttons"):
                yield Button("Yes", id="yes", variant="error")
                yield Button("No", id="no", variant="default")

    def on_mount(self):
        self._button_ids = ["yes", "no"]
        self._focused_index = 1
        self._focus_button(self._focused_index)

    def _focus_button(self, index):
        self._focused_index = index
        btn = self.query_one(f"#{self._button_ids[index]}", Button)
        btn.focus()

    def on_button_pressed(self, event):
        if event.button.id == "yes":
            self._notification_view_model.clear_notifications()
            try:
                self._view_model.delete_forest(self._forest_id)
                self._notification_view_model.add_notification(f"Forest '{self._forest_name}' deleted.", is_error=False)
                self._view_model.invalidate_data()
            except Exception as e:
                self._notification_view_model.add_notification(str(e), is_error=True)
            self.dismiss()
        else:
            self.dismiss()

    def on_key(self, event):
        match event.key:
            case "escape":
                self.dismiss(None)
                event.stop()
            case "left":
                if self._focused_index > 0:
                    self._focus_button(self._focused_index - 1)
                    event.stop()
            case "right":
                if self._focused_index < len(self._button_ids) - 1:
                    self._focus_button(self._focused_index + 1)
                    event.stop()
            case _:
                pass
