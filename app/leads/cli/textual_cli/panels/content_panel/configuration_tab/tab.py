import traceback

from partial_injector.partial_container import Container
from returns.result import safe
from spinq import dicts
from textual.app import ComposeResult
from textual.containers import Horizontal, Vertical
from textual.widgets import Static, Input
from textual import events

from leads.application_core.secondary_ports.exceptions import get_traceback
from leads.cli.textual_cli.panels.base_view import BaseView
from .view_model import ConfigurationViewModel, InitializedFocusState
from ...app_view_model import AppViewModel


class ConfigurationTab(BaseView):
    DEFAULT_CSS = """
    ConfigurationTab {
        width: 1fr;
        height: 1fr;
        padding: 0 0;
        content-align: left top;
    }

    ConfigurationTab > .table {
        width: 1fr;
        height: auto;   
    }

    ConfigurationTab > .table > .row {
        width: 1fr;
        height: 1;
        layout: horizontal;
    }

    ConfigurationTab > .table > .row > .cell-key {
        width: 40;
        padding: 0 1;
        color: #b0b0b0;
        background: #202020;
        border: none;
    }

    ConfigurationTab > .table > .row > .cell-value {
        width: 1fr;
        padding: 0 1;
        color: #b0b0b0;
        background: #202020;
        border: none;
    }

    ConfigurationTab > .table > .row.-selected > .cell-key,
    ConfigurationTab > .table > .row.-selected > .cell-value {
        background: #303030;
        color: #ffd787;
        text-style: bold;
    }
    
    ConfigurationTab:focus-within .table > .row > .cell-key,
    ConfigurationTab:focus-within .table > .row > .cell-value,
    ConfigurationTab:focus .table > .row > .cell-key,
    ConfigurationTab:focus .table > .row > .cell-value {
        background: #000000;
        color: #ffffff;
    }

    ConfigurationTab:focus-within .table > .row.-selected > .cell-key,
    ConfigurationTab:focus-within .table > .row.-selected > .cell-value,
    ConfigurationTab:focus .table > .row.-selected > .cell-key,
    ConfigurationTab:focus .table > .row.-selected > .cell-value {
        background: #202020;
        color: #ffd787;
    }

    ConfigurationTab > .table > .header {
        width: 1fr;
        height: 1;
        layout: horizontal;
        background: #333333;
    }
    ConfigurationTab > .table > .header > .cell-key {
        width: 24;
        padding: 0 1;
        color: #00eaff;
        background: #333333;
        text-style: bold;
        border: none;
    }
    ConfigurationTab > .table > .header > .cell-value {
        width: 1fr;
        padding: 0 1;
        color: #00eaff;
        background: #333333;
        text-style: bold;
        border: none;
    }
    """

    def __init__(self,
                 container: Container,
                 app_view_model: AppViewModel) -> None:
        super().__init__("Configuration", id="configuration-tab")

        self.container = container
        self.app_view_model = app_view_model

        self.view_model = ConfigurationViewModel(self.container,
                                                 self.app_view_model.notification_view_model,
                                                 self._on_view_model_changed)

        self._rows: list[Horizontal] = []
        self.can_focus = True

    def _on_view_model_changed(self):
        self.refresh(recompose=True)
        self.call_later(self.on_mount)

    def compose(self) -> ComposeResult:
        @safe
        def compose_configuration_layout(data: dict):
            with Vertical(classes="table"):
                with Horizontal(classes="header"):
                    yield Static("Key", classes="cell-key")
                    yield Static("Value", classes="cell-value")
                for idx, (k, v) in enumerate(data.__dict__.items()):
                    with Horizontal(classes="row"):
                        yield Static(k, classes="cell-key")
                        if self.view_model.edit_state.row_index == idx:
                            yield EditableInput(self.view_model.edit_state, value=self.view_model.edit_state.value or v,
                                                classes="cell-value", id=f"edit-input-{idx}")
                        else:
                            yield Static(v, classes="cell-value")

        @safe
        def compose_error_layout(error: Exception):
            yield Static(f"Error loading configuration: {str(error)}\n{get_traceback(error)}", classes="error-message")

        return (
            self.view_model.load_configuration()
            .bind(compose_configuration_layout)
            .lash(compose_error_layout)
            .unwrap()
        )

    def on_mount(self) -> None:
        self._rows = list(self.query(Horizontal).filter(".row"))
        self.view_model.focus_state = InitializedFocusState(self.view_model.focus_state.index if self.view_model.focus_state else 0,
                                                            total_rows=len(self._rows))
        self.apply_selection()

    def apply_selection(self) -> None:
        for i, row in enumerate(self._rows):
            row.set_class(i == self.view_model.focus_state.index, "-selected")

    def on_key(self, event) -> None:
        key = getattr(event, "key", None)
        match key:
            case "down":
                self.view_model.focus_state.move_next()
                self.apply_selection()
            case "up":
                self.view_model.focus_state.move_prev()
                self.apply_selection()
            case "i":
                idx = self.view_model.focus_state.index
                key, value = dicts.get_key_value_by_index_(self.view_model.data.__dict__, idx)
                self.view_model.edit_state.start(idx, key, value)


class EditableInput(Input):
    def __init__(self, editing_state, **kwargs):
        super().__init__(**kwargs)

        self.editing_state = editing_state

    def on_mount(self) -> None:
        self.focus()
        self.cursor_position = len(self.value)

    def on_key(self, event: events.Key) -> None:
        match event.key:
            case "escape":
                self.editing_state.end()
                event.stop()
            case "tab":
                self.editing_state.end()
            case "enter":
                self.editing_state.apply(self.value)
                event.stop()
