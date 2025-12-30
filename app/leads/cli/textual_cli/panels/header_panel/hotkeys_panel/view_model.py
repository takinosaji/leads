from dataclasses import dataclass
from typing import Callable

@dataclass
class HotkeyItem:
    symbol: str
    definition: str

class HotkeysViewModel:
    def __init__(self, notify_view: Callable[[], None]):
        self._notify_view = notify_view
        self.hotkeys: list[HotkeyItem] = []

    def _changed(self):
        if self._notify_view is not None:
            self._notify_view()

    def set_hotkeys(self, hotkeys: list[HotkeyItem]):
        self.hotkeys = hotkeys
        self._changed()

    def clear_hotkeys(self):
        self.hotkeys.clear()
        self._changed()

