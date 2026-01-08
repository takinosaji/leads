from typing import List, Optional
from rx.subject import BehaviorSubject
from returns.result import safe
from partial_injector.partial_container import Container

from leads.application_core.forests.services import ForestGetter
from leads.application_core.secondary_ports.forests import Forest


class ForestsFocusState:
    def __init__(self, row_index: int, col_index: int, total_rows: int, total_cols: int) -> None:
        self.total_rows = total_rows
        self.total_cols = total_cols

        if self.total_rows:
            self.row_index = row_index % self.total_rows
        else:
            self.row_index = 0

        if self.total_cols:
            self.col_index = col_index % self.total_cols
        else:
            self.col_index = 0

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
    def __init__(self, container: Container):
        self.__container: Container = container
        self.data: Optional[List[Forest]] = None
        self.focus_state: Optional[ForestsFocusState] = None
        self.include_archived: bool = False
        self._subject: BehaviorSubject = BehaviorSubject(None)

    def subscribe(self, observer):
        return self._subject.subscribe(observer)

    def clear_state(self) -> None:
        self.data = None
        self.focus_state = None

    def toggle_include_archived(self) -> None:
        self.include_archived = not self.include_archived
        self._subject.on_next(None)

    def notify_changed(self) -> None:
        self._subject.on_next(None)

    @safe
    def load_forests(self):
        if self.data is None:
            self.data = (
                self.__container.resolve(ForestGetter)(self.include_archived)
                .unwrap()
            )
        return self.data
