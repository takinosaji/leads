from dataclasses import dataclass
from typing import Callable

from returns.result import safe


@dataclass
class NotificationItem:
    message: str
    is_error: bool = False


class NotificationViewModel:
    def __init__(self, notify_view: Callable[[], None]):
        self._notify_view = notify_view
        self.notifications: list[NotificationItem] = []

    def _changed(self):
        if self._notify_view is not None:
            self._notify_view()

    @property
    def has_error(self) -> bool:
        return any(n.is_error for n in self.notifications)

    @safe
    def add_notification(self, item: NotificationItem):
        self.notifications.append(item)
        self._changed()

    @safe
    def clear_notifications(self):
        self.notifications.clear()
        self._changed()
