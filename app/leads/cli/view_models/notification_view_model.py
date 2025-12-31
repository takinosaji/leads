from dataclasses import dataclass
from rx.subject import BehaviorSubject
from returns.result import safe


@dataclass
class NotificationItem:
    message: str
    is_error: bool = False


class NotificationViewModel:
    def __init__(self):
        self._notifications_subject = BehaviorSubject([])

    @property
    def notifications(self) -> list[NotificationItem]:
        return self._notifications_subject.value

    @property
    def has_error(self) -> bool:
        return any(n.is_error for n in self.notifications)

    @safe
    def add_notification(self, message: str, is_error: bool = False):
        new_list = self.notifications + [NotificationItem(message, is_error)]
        self._notifications_subject.on_next(new_list)

    @safe
    def clear_notifications(self):
        self._notifications_subject.on_next([])

    def subscribe(self, callback):
        return self._notifications_subject.subscribe(lambda _: callback())
