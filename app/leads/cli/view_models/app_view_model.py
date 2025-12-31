from partial_injector.partial_container import Container
from typing import List
from rx.subject import BehaviorSubject
from textual.widget import Widget
from textual.screen import Screen

from leads.cli.view_models.notification_view_model import NotificationViewModel
from leads.cli.view_models.hotkeys_view_model import HotkeysViewModel
from leads.cli.view_models.menu_view_model import MenuViewModel, MenuItem
from leads.cli.views.models import CliTab
from leads.cli.view_models.configuration_view_model import ConfigurationViewModel


class AppFocusState:
    def __init__(self):
        self.focusable_widgets: List[Widget] = []
        self.index_subject = BehaviorSubject(0)

    @property
    def index(self):
        return self.index_subject.value

    def build(self, *widgets: Widget) -> None:
        self.focusable_widgets = [w for w in widgets if isinstance(w, Widget)]
        if self.focusable_widgets:
            self.index_subject.on_next(min(self.index, max(0, len(self.focusable_widgets) - 1)))

    def set_focus_at(self, screen: Screen, index: int) -> None:
        if not self.focusable_widgets:
            return
        index = index % len(self.focusable_widgets)
        screen.set_focus(self.focusable_widgets[index])

    def focus_next(self, screen: Screen) -> None:
        if not self.focusable_widgets:
            return
        new_index = (self.index + 1) % len(self.focusable_widgets)
        self.index_subject.on_next(new_index)


class AppViewModel:
    def __init__(self, container: Container):
        self.hotkeys_view_model = HotkeysViewModel()
        self.menu_view_model = MenuViewModel([
            MenuItem("Configuration", CliTab.CONFIGURATION),
            MenuItem("Forests", CliTab.FORESTS),
            MenuItem("Trails", CliTab.TRAILS)
        ])
        self.notification_view_model = NotificationViewModel()
        self.configuration_view_model = ConfigurationViewModel(container, self.notification_view_model)

        self.focus_state: AppFocusState = AppFocusState()
