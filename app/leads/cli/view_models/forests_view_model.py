from typing import List, Optional
from rx.subject import BehaviorSubject
from returns.result import safe
from partial_injector.partial_container import Container

from leads.application_core.forests.services import ForestsGetter, ForestCreator
from leads.application_core.secondary_ports.exceptions import get_traceback
from leads.application_core.secondary_ports.forests import Forest, NewForestDto
from leads.cli.view_models.notification_view_model import NotificationViewModel


class ForestsFocusState:
    def __init__(self, index: int, total_rows: int) -> None:
        self.total_rows = total_rows
        if self.total_rows:
            self.index = index % self.total_rows
        else:
            self.index = 0

    def move_next(self) -> None:
        if self.total_rows:
            self.index = (self.index + 1) % self.total_rows

    def move_prev(self) -> None:
        if self.total_rows:
            self.index = (self.index - 1) % self.total_rows


class ForestsViewModel:
    def __init__(self,
                 container: Container,
                 notification_view_model: NotificationViewModel):
        self.__container: Container = container
        self._notification_view_model: NotificationViewModel = notification_view_model
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
                self.__container.resolve(ForestsGetter)(self.include_archived)
                .unwrap()
            )
        return self.data

    @safe
    def create_forest(self, dto: NewForestDto) -> Forest:
        self._notification_view_model.clear_notifications() move out this to the tab, see how it works
        try:
            return self.__container.resolve(ForestCreator)(dto).unwrap()
        except Exception as error:
            self._notification_view_model.add_notification(str(error), True)
