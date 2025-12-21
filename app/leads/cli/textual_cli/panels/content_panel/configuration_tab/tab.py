from typing import Optional

from partial_injector.partial_container import Container
from returns.result import safe
from textual.app import ComposeResult
from textual.containers import Horizontal, Vertical
from textual.widgets import Static, Input
from textual import events

from leads.cli.configuration.factory import CliConfigurationLoader
from leads.cli.textual_cli.panels.content_panel.base_view import BaseView


type ConfigurationFocusState = InitializedFocusState | None


class InitializedFocusState:
    def __init__(self, index, total_rows: int) -> None:
        self.total_rows = total_rows
        self.index = index

    def move_next(self) -> None:
        if self.total_rows:
            self.index = (self.index + 1) % self.total_rows

    def move_prev(self) -> None:
        if self.total_rows:
            self.index = (self.index - 1) % self.total_rows


class EditingState:
    def __init__(self, parent):
        self.parent = parent
        self.row_index: int | None = None
        self.value: str = ""

    def start(self, idx: int, value: str):
        self.row_index = idx
        self.value = value
        self.parent.recompose_on_mount()

    def cancel(self):
        self.row_index = None
        self.value = ""
        self.parent.recompose_on_mount()

    @staticmethod
    def apply(widget, value):
        widget.post_message(Input.Submitted(widget, value))


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
        color: #ffffff;
        background: #202020;
        border: none;
    }

    ConfigurationTab > .table > .row > .cell-value {
        width: 1fr;
        padding: 0 1;
        color: #c0c0c0;
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

    def __init__(self, container: Container) -> None:
        super().__init__("Configuration", id="configuration-tab")

        self.container = container

        self.data: Optional[dict] = None

        self._rows: list[Horizontal] = []
        self.can_focus = True
        self.focus_state = None
        self.editing_state = EditingState(self)

    def compose(self) -> ComposeResult:
        @safe
        def load_configuration(cli_configuration):
            self.data = {
                'runtime_configuration.min_log_level': cli_configuration.runtime_configuration.min_log_level.name,
                'context_configuration.active_forest': cli_configuration.context_configuration.active_forest or ""
            }
            return self.data

        @safe
        def compose_configuration_layout(data: dict):
            with Vertical(classes="table"):
                with Horizontal(classes="header"):
                    yield Static("Key", classes="cell-key")
                    yield Static("Value", classes="cell-value")
                for idx, (k, v) in enumerate(data.items()):
                    with Horizontal(classes="row"):
                        yield Static(k, classes="cell-key")
                        if self.editing_state.row_index == idx:
                            yield EditableInput(self.editing_state, value=self.editing_state.value or v,
                                                classes="cell-value", id=f"edit-input-{idx}")
                        else:
                            yield Static(v, classes="cell-value")

        @safe
        def compose_error_layout(error: Exception):
            yield Static(f"Error loading configuration: {str(error)}", classes="error-message")

        return (
            self.container.resolve(CliConfigurationLoader)()
            .bind(load_configuration)
            .bind(compose_configuration_layout)
            .lash(compose_error_layout)
            .unwrap()
        )

    def recompose_on_mount(self):
        self.refresh(recompose=True)
        self.call_later(self.on_mount)

    def on_mount(self) -> None:
        self._rows = list(self.query(Horizontal).filter(".row"))
        self.focus_state = InitializedFocusState(self.focus_state.index if self.focus_state else 0,
                                                 total_rows=len(self._rows))
        self.apply_selection()

    def apply_selection(self) -> None:
        for i, row in enumerate(self._rows):
            row.set_class(i == self.focus_state.index, "-selected")

    def on_key(self, event) -> None:
        key = getattr(event, "key", None)
        match key:
            case "down":
                self.focus_state.move_next()
                self.apply_selection()
            case "up":
                self.focus_state.move_prev()
                self.apply_selection()
            case "i":
                idx = self.focus_state.index
                self.editing_state.start(idx, list(self.data.values())[idx])

    def on_input_submitted(self, event: Input.Submitted) -> None:
        if self.editing_state.row_index is not None:
            self.__save_configuration_data(event.value)

    def __save_configuration_data(self, new_value) -> None:
        #self.editing_state.row_index
        self.recompose_on_mount()


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
                self.editing_state.cancel()
                event.stop()
            case "tab":
                self.editing_state.cancel()
            case "enter":
                self.editing_state.apply(self, self.value)
                event.stop()
