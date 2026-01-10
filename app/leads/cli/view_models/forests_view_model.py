from typing import List, Optional
from rx.subject import BehaviorSubject
from returns.result import safe
from partial_injector.partial_container import Container

from leads.application_core.forests.services import ForestsGetter, ForestCreator, ForestUpdater
from leads.application_core.secondary_ports.forests import Forest, NewForestDto
from leads.cli.view_models.notification_view_model import NotificationViewModel


class ForestsFocusState:
    def __init__(self, index: int, total_rows: int) -> None:
        self.total_rows = total_rows
        if self.total_rows:
            self.index = index % self.total_rows
        else:
            self.index = 0
        self._subject = BehaviorSubject(self.index)

    def move_next(self) -> None:
        if self.total_rows:
            self.index = (self.index + 1) % self.total_rows
            self._subject.on_next(self.index)

    def move_prev(self) -> None:
        if self.total_rows:
            self.index = (self.index - 1) % self.total_rows
            self._subject.on_next(self.index)

    def subscribe(self, callback):
        return self._subject.subscribe(callback)


class ForestsViewModel:
    def __init__(self,
                 container: Container,
                 notification_view_model: NotificationViewModel):
        self.__container: Container = container
        self._notification_view_model: NotificationViewModel = notification_view_model
        self.data: Optional[List[Forest]] = None
        self.focus_state: Optional[ForestsFocusState] = None
        self.selected_forest: Optional[Forest] = None
        self._include_archived: bool = False
        self._subject: BehaviorSubject = BehaviorSubject(None)
        self._focus_subscription = None

    def subscribe(self, observer):
        return self._subject.subscribe(observer)

    def set_focus_state(self, index: int, total_rows: int):
        if self._focus_subscription:
            self._focus_subscription.dispose()

        self.selected_forest = None
        self.focus_state = ForestsFocusState(index, total_rows)

        self._focus_subscription = self.focus_state.subscribe(self._on_focus_index_changed)
        self._on_focus_index_changed(self.focus_state.index)

    def _on_focus_index_changed(self, index: int):
        if self.data and 0 <= index < len(self.data):
            self.selected_forest = self.data[index]
        else:
            self.selected_forest = None

    def clear_state(self) -> None:
        self.data = None
        self.focus_state = None
        self.selected_forest = None
        if self._focus_subscription:
            self._focus_subscription.dispose()
            self._focus_subscription = None

    def toggle_include_archived(self) -> None:
        self._include_archived = not self._include_archived
        self._subject.on_next(None)

    def notify_changed(self) -> None:
        self._subject.on_next(None)

    @safe
    def load_forests(self):
        if self.data is None:
            self.data = (
                self.__container.resolve(ForestsGetter)(self._include_archived)
                .unwrap()
            )
            self.data.sort(key=lambda f: (f.updated_at is not None, f.updated_at), reverse=True)
        return self.data

    def create_forest(self, dto: NewForestDto) -> Forest:
        return self.__container.resolve(ForestCreator)(dto)

    def update_forest(self, dto: NewForestDto) -> Forest:
        return self.__container.resolve(ForestUpdater)(dto)
