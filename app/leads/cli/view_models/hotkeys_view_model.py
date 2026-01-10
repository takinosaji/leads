from dataclasses import dataclass
from rx.subject import BehaviorSubject

@dataclass
class HotkeyItem:
    symbol: str
    definition: str

class HotkeysViewModel:
    def __init__(self):
        self._hotkeys_subject = BehaviorSubject([])

    @property
    def hotkeys(self) -> list[list[HotkeyItem]]:
        return self._hotkeys_subject.value

    def set_hotkeys(self, *columns: list[HotkeyItem]):
        self._hotkeys_subject.on_next(list(columns))

    def clear_hotkeys(self):
        self._hotkeys_subject.on_next([])

    def subscribe(self, callback):
        return self._hotkeys_subject.subscribe(lambda _: callback())
