from textual.app import ComposeResult
from textual.containers import Container

from leads.cli.textual_cli.models import CliTab
from leads.cli.textual_cli.panels.app_view_model import AppViewModel
from leads.cli.textual_cli.panels.base_view import BaseView
from leads.cli.textual_cli.panels.content_panel.configuration_tab.tab import ConfigurationTab


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
                 container: Container,
                 app_view_model: AppViewModel,
                 **kwargs) -> None:
        super().__init__(**kwargs)

        self.container = container
        self.app_view_model: AppViewModel = app_view_model

        self._tabs: dict[CliTab, BaseView] = {}
        self.__active_tab_key: CliTab = CliTab.CONFIGURATION
        self.can_focus = True

    def activate(self, tab_key_to_activate: CliTab) -> None:
        self.__active_tab_key = tab_key_to_activate

        for tab_key, tab_view in self._tabs.items():
            if tab_key == tab_key_to_activate:
                tab_view.remove_class("hidden")
                tab_view.activate()
            else:
                tab_view.add_class("hidden")
                tab_view.deactivate()

        self.app_view_model.notification_view_model.clear_notifications()

    def compose(self) -> ComposeResult:
        cfg = ConfigurationTab(self.container, self.app_view_model)

        frs = BaseView("Forests", id="forests-tab")
        trl = BaseView("Trails", id="trails-tab")
        self._tabs = {
            CliTab.CONFIGURATION: cfg,
            CliTab.FORESTS: frs,
            CliTab.TRAILS: trl,
        }

        #self.set_active(self.__active_tab_key)

        yield cfg
        yield frs
        yield trl

    def on_focus(self, event) -> None:
        active_tab = self._tabs.get(self.__active_tab_key)
        if active_tab and hasattr(active_tab, 'focus'):
            active_tab.focus()

    def send_command_to_active_tab(self, text: str) -> bool:
        active_tab = self._tabs.get(self.__active_tab_key)
        if not active_tab:
            return False

        return active_tab.handle_command(text)
