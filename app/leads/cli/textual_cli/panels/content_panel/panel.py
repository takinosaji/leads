from __future__ import annotations

from textual.app import ComposeResult
from textual.containers import Container
from textual.widgets import Static

from leads.cli.textual_cli.models import CliTab


class _BaseView(Container):
    DEFAULT_CSS = """
    _BaseView {
        width: 1fr;
        height: 1fr;
        content-align: center middle;
    }
    """

    def __init__(self, title: str, *, id: str | None = None) -> None:
        super().__init__(id=id, classes="content-tab")
        self._title = title

    def compose(self) -> ComposeResult:
        yield Static(self._title)


class ConfigurationTab(_BaseView):
    def __init__(self) -> None:
        super().__init__("Configuration", id="configuration-tab")


class ForestsTab(_BaseView):
    def __init__(self) -> None:
        super().__init__("Forests", id="forests-tab")


class TrailsTab(_BaseView):
    def __init__(self) -> None:
        super().__init__("Trails", id="trails-tab")


class ContentPanel(Container):
    DEFAULT_CSS = """
    ContentPanel {
        width: 1fr;
        height: 1fr;
        background: #000000;
        border: round #268bd2;
        padding: 1 1;
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
        self.__views: dict[CliTab, _BaseView] = {}
        self.__active_tab: CliTab = CliTab.CONFIGURATION

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
        frs = ForestsTab()
        trl = TrailsTab()
        self.__views = {
            CliTab.CONFIGURATION: cfg,
            CliTab.FORESTS: frs,
            CliTab.TRAILS: trl,
        }

        self.__apply_active(self.__active_tab)

        yield cfg
        yield frs
        yield trl

    def set_active(self, label: CliTab | str) -> None:
        if label == self.__active_tab:
            return
        self.__apply_active(label)
