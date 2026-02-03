from __future__ import annotations
from dataclasses import dataclass

from rx.subject import BehaviorSubject


@dataclass(frozen=True)
class StatePanelState:
    active_forest: str = ""
    active_trail: str = ""


class StatePanelViewModel:
    def __init__(self) -> None:
        self._subject: BehaviorSubject = BehaviorSubject(StatePanelState())

    @property
    def state(self) -> StatePanelState:
        return self._subject.value

    @property
    def active_forest(self) -> str:
        return self.state.active_forest

    @property
    def active_trail(self) -> str:
        return self.state.active_trail


    def subscribe(self, callback):
        return self._subject.subscribe(lambda _: callback())
