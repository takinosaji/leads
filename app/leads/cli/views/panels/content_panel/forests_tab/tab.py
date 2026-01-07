from typing import List

from textual.app import ComposeResult
from textual.containers import Vertical, Horizontal
from textual.widgets import Static

from leads.application_core.secondary_ports.forests import Forest
from leads.cli.views.panels.base_view import BaseView
from leads.cli.view_models.hotkeys_view_model import HotkeyItem, HotkeysViewModel
from leads.cli.view_models.forests_view_model import ForestsViewModel, ForestsFocusState


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

    ForestsTab > .table > .row > .cell.-selected {
        background: #303030;
        color: #ffd787;
        text-style: bold;
    }

    ForestsTab:focus-within .table > .row > .cell-id,
    ForestsTab:focus-within .table > .row > .cell-name,
    ForestsTab:focus-within .table > .row > .cell-description,
    ForestsTab:focus .table > .row > .cell-id,
    ForestsTab:focus .table > .row > .cell-name,
    ForestsTab:focus .table > .row > .cell-description {
        background: #000000;
        color: #ffffff;
    }

    ForestsTab:focus-within .table > .row > .cell.-selected,
    ForestsTab:focus .table > .row > .cell.-selected {
        background: #202020;
        color: #ffd787;
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
    """

    def __init__(self,
                 forests_view_model: ForestsViewModel,
                 hotkeys_view_model: HotkeysViewModel):
        super().__init__("Forests", id="forests-tab")
        self._view_model = forests_view_model
        self._hotkeys_view_model = hotkeys_view_model
        self._rows: List[Horizontal] = []
        self._cells: List[List[Static]] = []
        self.can_focus = True
        self._is_selected = False
        self._subscription = self._view_model.subscribe(lambda _: self._on_view_model_changed())

    def on_focus(self, event) -> None:
        self._hotkeys_view_model.set_hotkeys([
            HotkeyItem("<Tab>", "Change Focus"),
            HotkeyItem("<↑/↓/←/→>", "Navigate Cells"),
        ])
        return None

    def on_mount(self) -> None:
        if not self._is_selected:
            return None
        self._rows = list(self.query(Horizontal).filter(".row"))
        self._cells = []
        for row in self._rows:
            row_cells: List[Static] = []
            for cls in ("cell-id", "cell-name", "cell-description"):
                cell = row.query_one(f".{cls}", Static)
                cell.add_class("cell")
                row_cells.append(cell)
            self._cells.append(row_cells)
        total_rows = len(self._cells)
        total_cols = len(self._cells[0]) if total_rows and self._cells[0] else 0
        if total_rows and total_cols:
            index_row = self._view_model.focus_state.row_index if self._view_model.focus_state else 0
            index_col = self._view_model.focus_state.col_index if self._view_model.focus_state else 0
            self._view_model.focus_state = ForestsFocusState(index_row, index_col, total_rows, total_cols)
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
        if not self._view_model.focus_state:
            return None
        for r, row_cells in enumerate(self._cells):
            for c, cell in enumerate(row_cells):
                cell.set_class(self._view_model.focus_state.row_index == r and
                               self._view_model.focus_state.col_index == c, "-selected")
        return None

    def on_key(self, event) -> None:
        key = getattr(event, "key", None)
        if not self._view_model.focus_state:
            return None
        match key:
            case "down":
                self._view_model.focus_state.move_next_row()
            case "up":
                self._view_model.focus_state.move_prev_row()
            case "right":
                self._view_model.focus_state.move_next_col()
            case "left":
                self._view_model.focus_state.move_prev_col()
            case _:
                return None
        self.apply_selection()
        return None

    def compose(self) -> ComposeResult:
        if not self._is_selected:
            yield Static("Forests Tab (not selected)", classes="info-message")
            return None

        with Vertical(classes="table"):
            with Horizontal(classes="header"):
                yield Static("ID", classes="cell-id")
                yield Static("NAME", classes="cell-name")
                yield Static("DESCRIPTION", classes="cell-description")
            if self._view_model.data:
                for forest in self._view_model.data:
                    with Horizontal(classes="row"):
                        yield Static(forest.id, classes="cell-id")
                        yield Static(forest.name, classes="cell-name")
                        yield Static(forest.description or "", classes="cell-description")
        return None

    def handle_command(self, text: str) -> bool:
        return False
