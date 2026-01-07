from typing import List, Optional
from rx.subject import BehaviorSubject

from leads.application_core.secondary_ports.forests import Forest


class ForestsFocusState:
    def __init__(self, row_index: int = 0, col_index: int = 0,
                 total_rows: int = 0, total_cols: int = 0) -> None:
        self.row_index = row_index
        self.col_index = col_index
        self.total_rows = total_rows
        self.total_cols = total_cols

    def move_next_row(self) -> None:
        if self.total_rows:
            self.row_index = (self.row_index + 1) % self.total_rows

    def move_prev_row(self) -> None:
        if self.total_rows:
            self.row_index = (self.row_index - 1) % self.total_rows

    def move_next_col(self) -> None:
        if self.total_cols:
            self.col_index = (self.col_index + 1) % self.total_cols

    def move_prev_col(self) -> None:
        if self.total_cols:
            self.col_index = (self.col_index - 1) % self.total_cols


class ForestsViewModel:
    def __init__(self) -> None:
        self.data: Optional[List[Forest]] = None
        self.focus_state: Optional[ForestsFocusState] = None
        self._subject: BehaviorSubject = BehaviorSubject(None)

    def subscribe(self, observer):
        return self._subject.subscribe(observer)

    def clear_state(self) -> None:
        self.data = None
        self.focus_state = None

    def notify_changed(self) -> None:
        self._subject.on_next(None)

