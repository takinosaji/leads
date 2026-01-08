from partial_injector.partial_container import Container
from typing import List
from textual.widget import Widget
from textual.screen import Screen

from leads.cli.view_models.notification_view_model import NotificationViewModel
from leads.cli.view_models.hotkeys_view_model import HotkeysViewModel
from leads.cli.view_models.menu_view_model import MenuViewModel, MenuItem
from leads.cli.views.models import CliTab
from leads.cli.view_models.configuration_view_model import ConfigurationViewModel
from leads.cli.view_models.forests_view_model import ForestsViewModel


class AppFocusState:
    def __init__(self, screen: Screen):
        self._focusable_widgets: List[Widget] = []
        self._screen = screen
        self._index: int = 0

    def build(self, *widgets: Widget) -> None:
        self._focusable_widgets = [w for w in widgets if isinstance(w, Widget)]
        if self._focusable_widgets:
            self._index = min(self._index, max(0, len(self._focusable_widgets) - 1))

    def focus_next(self) -> None:
        focused_widget = self._focusable_widgets[self._index]
        num_widgets = len(self._focusable_widgets)
        for i in range(1, num_widgets + 1):
            new_index = (self._index + i) % num_widgets
            widget = self._focusable_widgets[new_index]
            if not widget.has_class("hidden") and widget is not focused_widget:
                self._index = new_index
                self.set_focus_widget(widget)
                break
        return None

    def set_focus_widget(self, widget: Widget) -> None:
        if widget in self._focusable_widgets:
            self._screen.set_focus(widget)
            self._index = self._focusable_widgets.index(widget)

    def sync_focus_widget(self, widget: Widget) -> None:
        self._index = self._focusable_widgets.index(widget)

    def try_sync_focus_widget(self, widget: Widget) -> None:
        if widget in self._focusable_widgets:
            self._index = self._focusable_widgets.index(widget)

    def refocus(self) -> None:
        self.set_focus_widget(self._focusable_widgets[self._index])


class AppViewModel:
    def __init__(self,
                 container: Container,
                 screen: Screen):
        self.hotkeys_view_model = HotkeysViewModel()
        self.menu_view_model = MenuViewModel([
            MenuItem("Configuration", CliTab.CONFIGURATION),
            MenuItem("Forests", CliTab.FORESTS),
            MenuItem("Trails", CliTab.TRAILS)
        ])
        self.notification_view_model = NotificationViewModel()
        self.configuration_view_model = ConfigurationViewModel(container, self.notification_view_model)
        self.forests_view_model = ForestsViewModel(container)

        self.focus_state: AppFocusState = AppFocusState(screen)
