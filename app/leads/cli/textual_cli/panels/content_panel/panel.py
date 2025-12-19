from textual.app import ComposeResult
from textual.containers import Container

from leads.cli.textual_cli.models import CliTab
from leads.cli.textual_cli.panels.content_panel.base_view import BaseView
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

    def __init__(self, **kwargs) -> None:
        super().__init__(**kwargs)
        self.__views: dict[CliTab, BaseView] = {}
        self.__active_tab: CliTab = CliTab.CONFIGURATION
        self.can_focus = True

    def __apply_active(self, tab: CliTab) -> None:
        if not self.__views:
            self.__active_tab = tab
            return
        for name, view in self.__views.items():
            if name == tab:
                view.remove_class("hidden")
                self.__active_tab = name
            else:
                view.add_class("hidden")

    def compose(self) -> ComposeResult:
        cfg = ConfigurationTab()

        frs = BaseView("Forests", id="forests-tab")
        trl = BaseView("Trails", id="trails-tab")
        self.__views = {
            CliTab.CONFIGURATION: cfg,
            CliTab.FORESTS: frs,
            CliTab.TRAILS: trl,
        }

        self.__apply_active(self.__active_tab)

        yield cfg
        yield frs
        yield trl

    def set_active(self, tab: CliTab | str) -> None:
        if tab == self.__active_tab:
            return
        self.__apply_active(tab)

    def on_focus(self, event) -> None:
        active_view = self.__views.get(self.__active_tab)
        if active_view and hasattr(active_view, 'focus'):
            active_view.focus()
