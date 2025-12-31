from rx.subject import BehaviorSubject
from leads.cli.views.models import CliTab
from dataclasses import dataclass
from typing import List

@dataclass(frozen=True)
class MenuItem:
    text: str
    tab: CliTab

class MenuViewModel:
    def __init__(self, menu_items: List[MenuItem]):
        self.menu_items = menu_items
        self.selected_index_subject = BehaviorSubject(0)

    @property
    def selected_index(self):
        return self.selected_index_subject.value

    def set_selected_index(self, index: int):
        self.selected_index_subject.on_next(index)

    def get_selected_item(self):
        return self.menu_items[self.selected_index]
