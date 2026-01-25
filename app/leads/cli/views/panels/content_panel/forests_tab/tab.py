from typing import List

from returns.result import safe
from textual.app import ComposeResult
from textual.containers import Vertical, Horizontal
from textual.widgets import Static
from leads.application_core.secondary_ports.exceptions import get_traceback
from leads.cli.formatting import escape_textual

from leads.cli.view_models.app_view_model import AppFocusState
from leads.cli.view_models.notification_view_model import NotificationViewModel
from leads.cli.views.panels.base_view import BaseView
from leads.cli.view_models.hotkeys_view_model import HotkeyItem, HotkeysViewModel
from leads.cli.view_models.forests_view_model import ForestsViewModel
from leads.cli.views.panels.content_panel.forests_tab.forest_creation_modal import ForestCreationModal
from leads.cli.views.panels.content_panel.forests_tab.forest_update_modal import ForestUpdateModal
from leads.cli.views.panels.content_panel.forests_tab.forest_deletion_modal import ForestDeletionModal


class ForestsTab(BaseView):
    DEFAULT_CSS = """
    ForestsTab {
        width: 1fr;
        height: 1fr;
        padding: 0 0;
        content-align: left top;
    }

    ForestsTab > .table {
        width: 1fr;
        height: auto;
    }

    ForestsTab > .table > .row {
        width: 1fr;
        height: 1;
        layout: horizontal;
    }

    ForestsTab > .table > .row > .cell-id {
        width: 20;
        padding: 0 1;
        color: #b0b0b0;
        background: #202020;
        border: none;
    }

    ForestsTab > .table > .row > .cell-name {
        width: 30;
        padding: 0 1;
        color: #b0b0b0;
        background: #202020;
        border: none;
    }

    ForestsTab > .table > .row > .cell-description {
        width: 1fr;
        padding: 0 1;
        color: #b0b0b0;
        background: #202020;
        border: none;
    }

    ForestsTab > .table > .row > .cell-created-at {
        width: 20;
        padding: 0 1;
        color: #b0b0b0;
        background: #202020;
        border: none;
    }

    ForestsTab > .table > .row > .cell-updated-at {
        width: 20;
        padding: 0 1;
        color: #b0b0b0;
        background: #202020;
        border: none;
    }

    ForestsTab > .table > .row > .cell-archived {
        width: 16;
        padding: 0 1;
        color: #b0b0b0;
        background: #202020;
        border: none;
    }

    ForestsTab > .table > .row.-selected > .cell-id,
    ForestsTab > .table > .row.-selected > .cell-name,
    ForestsTab > .table > .row.-selected > .cell-description,
    ForestsTab > .table > .row.-selected > .cell-created-at,
    ForestsTab > .table > .row.-selected > .cell-updated-at,
    ForestsTab > .table > .row.-selected > .cell-archived {
        background: #303030;
        color: #ffd787;
        text-style: bold;
    }

    ForestsTab > .table > .row > .cell.-selected {
        background: #505050;
        color: #ffd787;
        text-style: bold;
    }

    ForestsTab:focus-within .table > .row > .cell-id,
    ForestsTab:focus-within .table > .row > .cell-name,
    ForestsTab:focus-within .table > .row > .cell-description,
    ForestsTab:focus-within .table > .row > .cell-created-at,
    ForestsTab:focus-within .table > .row > .cell-updated-at,
    ForestsTab:focus-within .table > .row > .cell-archived,
    ForestsTab:focus .table > .row > .cell-id,
    ForestsTab:focus .table > .row > .cell-name,
    ForestsTab:focus .table > .row > .cell-description,
    ForestsTab:focus .table > .row > .cell-created-at,
    ForestsTab:focus .table > .row > .cell-updated-at,
    ForestsTab:focus .table > .row > .cell-archived {
        background: #000000;
        color: #ffffff;
    }

    ForestsTab:focus-within .table > .row.-selected > .cell-id,
    ForestsTab:focus-within .table > .row.-selected > .cell-name,
    ForestsTab:focus-within .table > .row.-selected > .cell-description,
    ForestsTab:focus-within .table > .row.-selected > .cell-created-at,
    ForestsTab:focus-within .table > .row.-selected > .cell-updated-at,
    ForestsTab:focus-within .table > .row.-selected > .cell-archived,
    ForestsTab:focus .table > .row.-selected > .cell-id,
    ForestsTab:focus .table > .row.-selected > .cell-name,
    ForestsTab:focus .table > .row.-selected > .cell-description,
    ForestsTab:focus .table > .row.-selected > .cell-created-at,
    ForestsTab:focus .table > .row.-selected > .cell-updated-at,
    ForestsTab:focus .table > .row.-selected > .cell-archived {
        background: #202020;
        color: #ffd787;
    }

    ForestsTab:focus-within .table > .row > .cell.-selected,
    ForestsTab:focus .table > .row > .cell.-selected {
        background: #404040;
        color: #ffd787;
        text-style: bold;
    }

    ForestsTab > .table > .header {
        width: 1fr;
        height: 1;
        layout: horizontal;
        background: #333333;
    }

    ForestsTab > .table > .header > .cell-id {
        width: 20;
        padding: 0 1;
        color: #00eaff;
        background: #333333;
        text-style: bold;
        border: none;
    }

    ForestsTab > .table > .header > .cell-name {
        width: 30;
        padding: 0 1;
        color: #00eaff;
        background: #333333;
        text-style: bold;
        border: none;
    }

    ForestsTab > .table > .header > .cell-description {
        width: 1fr;
        padding: 0 1;
        color: #00eaff;
        background: #333333;
        text-style: bold;
        border: none;
    }

    ForestsTab > .table > .header > .cell-created-at {
        width: 20;
        padding: 0 1;
        color: #00eaff;
        background: #333333;
        text-style: bold;
        border: none;
    }

    ForestsTab > .table > .header > .cell-updated-at {
        width: 20;
        padding: 0 1;
        color: #00eaff;
        background: #333333;
        text-style: bold;
        border: none;
    }

    ForestsTab > .table > .header > .cell-archived {
        width: 16;
        padding: 0 1;
        color: #00eaff;
        background: #333333;
        text-style: bold;
        border: none;
    }
    """

    def __init__(self,
                 forests_view_model: ForestsViewModel,
                 notification_view_model: NotificationViewModel,
                 hotkeys_view_model: HotkeysViewModel,
                 app_focus_state: AppFocusState):
        super().__init__("Forests", id="forests-tab")
        self._view_model = forests_view_model
        self._notification_view_model = notification_view_model
        self._hotkeys_view_model = hotkeys_view_model
        self._app_focus_state = app_focus_state
        self._rows: List[Horizontal] = []
        self.can_focus = True
        self._is_selected = False
        self._subscription = self._view_model.subscribe(lambda _: self._on_view_model_changed())

    def on_focus(self, event) -> None:
        self._hotkeys_view_model.set_hotkeys([
            HotkeyItem("<Tab>", "Change Focus"),
            HotkeyItem("<↑/↓>", "Navigate Forests"),
            HotkeyItem("<t/T>", "Toggle Archived"),
            HotkeyItem('<n/N>', "Create Forest"),
            HotkeyItem('<e/E>', "Edit Forest"),
            HotkeyItem('Del', "Delete Forest")
        ],
        [
            HotkeyItem("<a>", "Set as Active"),
            HotkeyItem("<A>", "Archive Forest"),
            HotkeyItem("<U>", "Unarchive Forest")
        ])
        return None

    def on_mount(self) -> None:
        if not self._is_selected:
            return None

        self._rows = list(self.query(Horizontal).filter(".row"))
        total_rows = len(self._rows)
        prev_index = self._view_model.focus_state.index if self._view_model.focus_state else 0
        self._view_model.set_focus_state(prev_index, total_rows)

        self.apply_selection()
        return None

    def on_unmount(self) -> None:
        self._subscription.dispose()

    def _on_view_model_changed(self):
        self.refresh(recompose=True)
        self.call_later(self.on_mount)

    def activate(self):
        self._is_selected = True
        self._on_view_model_changed()

    def deactivate(self):
        self._is_selected = False
        self._view_model.clear_state()
        self._on_view_model_changed()

    def apply_selection(self) -> None:
        for i, row in enumerate(self._rows):
            row.set_class(i == self._view_model.focus_state.index, "-selected")
        return None

    def on_key(self, event) -> None:
        key = getattr(event, "key", None)
        match key:
            case "down":
                self._view_model.focus_state.move_next()
                self.apply_selection()
            case "up":
                self._view_model.focus_state.move_prev()
                self.apply_selection()
            case _ if key in ("t", "T"):
                self._view_model.toggle_include_archived()
            case _ if key in ("n", "N"):
                self.call_later(self._open_forest_creation_modal)
            case _ if key in ("e", "E"):
                self.call_later(self._open_forest_update_modal)
            case _ if key in ("delete", "Delete", "del", "Del"):
                self.call_later(self._open_forest_deletion_modal)
            case "A":
                self._view_model.archive_selected_forest()
            case "U":
                self._view_model.unarchive_selected_forest()
        return None

    def _open_forest_creation_modal(self):
        self.app.push_screen(ForestCreationModal(self._view_model,
                                                 self._notification_view_model))

    def _open_forest_update_modal(self):
        if self._view_model.selected_forest:
            self.app.push_screen(ForestUpdateModal(self._view_model,
                                                   self._notification_view_model))

    def _open_forest_deletion_modal(self):
        if self._view_model.selected_forest:
            self.app.push_screen(ForestDeletionModal(self._view_model,
                                                     self._notification_view_model))

    def compose(self) -> ComposeResult:
        if not self._is_selected:
            yield Static("Forests Tab (not selected)", classes="info-message")
            return None

        @safe
        def compose_forests_layout(data):
            with Vertical(classes="table"):
                with Horizontal(classes="header"):
                    yield Static("ID", classes="cell-id")
                    yield Static("NAME", classes="cell-name")
                    yield Static("DESCRIPTION", classes="cell-description")
                    yield Static("CREATED AT", classes="cell-created-at")
                    yield Static("UPDATED AT", classes="cell-updated-at")
                    if self._view_model._include_archived:
                        yield Static("IS ARCHIVED", classes="cell-archived")
                if data:
                    for forest in data:
                        with Horizontal(classes="row"):
                            yield Static(forest.id, classes="cell-id")
                            yield Static(forest.name, classes="cell-name")
                            yield Static(forest.description or "", classes="cell-description")
                            yield Static(str(forest.created_at) if forest.created_at else "", classes="cell-created-at")
                            yield Static(str(forest.updated_at) if forest.updated_at else "", classes="cell-updated-at")
                            if self._view_model._include_archived:
                                yield Static("YES" if forest.is_archived else "NO", classes="cell-archived")

        @safe
        def compose_error_layout(error: Exception):
            error_text = escape_textual(str(error))
            traceback_text = escape_textual(get_traceback(error))
            yield Static(
                f"Error loading forests: {error_text}\n{traceback_text}",
                classes="error-message",
            )

        yield from (
            self._view_model.load_forests()
            .bind(compose_forests_layout)
            .lash(compose_error_layout)
            .unwrap()
        )
        return None

    def handle_command(self, text: str) -> bool:
        return False

    def on_click(self, event) -> None:
        if self._app_focus_state:
            self._app_focus_state.sync_focus_widget(self)
        return None

