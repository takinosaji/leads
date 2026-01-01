from textual.app import ComposeResult
from textual.containers import Container

from leads.cli.view_models.app_view_model import AppFocusState
from leads.cli.views.models import CliTab
from leads.cli.views.panels.base_view import BaseView
from leads.cli.views.panels.content_panel.configuration_tab.tab import ConfigurationTab
from leads.cli.view_models.hotkeys_view_model import HotkeysViewModel
from leads.cli.view_models.configuration_view_model import ConfigurationViewModel
from leads.cli.view_models.notification_view_model import NotificationViewModel


class ContentPanel(Container):
    DEFAULT_CSS = """
    ContentPanel {
        width: 1fr;
        height: 1fr;
        background: #202020;
        border: round #268bd2;
        padding: 0 0;
    }

    ContentPanel:focus,
    ContentPanel:focus-within {
        background: #000000;
    }

    ContentPanel > .hidden {
        display: none;
    }

    ContentPanel > .content-tab {
        width: 1fr;
        height: 1fr;
    }
    """

    def __init__(self,
                 configuration_view_model: ConfigurationViewModel,
                 hotkeys_view_model: HotkeysViewModel,
                 notification_view_model: NotificationViewModel,
                 **kwargs):
        super().__init__(id="content", **kwargs)
        self.can_focus = False

        self._configuration_view_model = configuration_view_model
        self._hotkeys_view_model = hotkeys_view_model
        self._notification_view_model = notification_view_model
        self.__active_tab_key: CliTab | None = None

        self.tabs = {
            CliTab.CONFIGURATION: ConfigurationTab(self._configuration_view_model, self._hotkeys_view_model),
            CliTab.FORESTS: BaseView("Forests", id="forests-tab"),
            CliTab.TRAILS: BaseView("Trails", id="trails-tab"),
        }

    def activate(self, tab_key_to_activate: CliTab) -> None:
        self.__active_tab_key = tab_key_to_activate

        for tab_key, tab_view in self.tabs.items():
            if tab_key == tab_key_to_activate:
                tab_view.remove_class("hidden")
                tab_view.activate()
            else:
                tab_view.add_class("hidden")
                tab_view.deactivate()

        self._notification_view_model.clear_notifications()

    def compose(self) -> ComposeResult:
        for tab in self.tabs.values():
            yield tab

    def send_command_to_active_tab(self, text: str) -> bool:
        active_tab = self.tabs.get(self.__active_tab_key)
        if not active_tab:
            return False

        return active_tab.handle_command(text)
