from partial_injector.partial_container import Container
from textual.app import App, ComposeResult
from textual.screen import Screen
from textual.containers import Horizontal, Vertical
from textual.events import Key

from leads.cli.view_models.app_view_model import AppViewModel
from leads.cli.views.models import CliTab
from leads.cli.views.panels.header_panel.panel import HeaderPanel
from leads.cli.views.panels.menu_panel.panel import MenuPanel, MenuSelectionChanged
from leads.cli.views.panels.content_panel.panel import ContentPanel
from leads.cli.views.panels.command_panel.panel import CommandPanel, CommandSubmitted, CommandPanelClosed
from leads.cli.views.panels.notification_panel.panel import NotificationPanel


class CliAppScreen(Screen):
    def __init__(self, container: Container):
        super().__init__()

        self.container = container

        self.app_view_model = AppViewModel(container, self)

        self.header_panel = HeaderPanel(self.app_view_model.hotkeys_view_model)
        self.menu_panel = MenuPanel(self.app_view_model.menu_view_model,
                                    self.app_view_model.hotkeys_view_model,
                                    self.app_view_model.focus_state)
        self.content_panel = ContentPanel(
            self.app_view_model.configuration_view_model,
            self.app_view_model.forests_view_model,
            self.app_view_model.hotkeys_view_model,
            self.app_view_model.notification_view_model,
            self.app_view_model.focus_state
        )
        self.notification_panel = NotificationPanel(self.app_view_model.notification_view_model)
        self.command_panel = CommandPanel(self.app_view_model.hotkeys_view_model)

    def compose(self) -> ComposeResult:
        with Vertical(id="main-vertical"):
            yield self.header_panel
            with Horizontal(id="body"):
                yield self.menu_panel
                yield self.content_panel
            yield self.command_panel
            yield self.notification_panel

    def on_mount(self) -> None:
        self.app_view_model.focus_state.build(self.menu_panel,
                                              self.content_panel.tabs[CliTab.CONFIGURATION],
                                              self.content_panel.tabs[CliTab.FORESTS],
                                              self.content_panel.tabs[CliTab.TRAILS])
        self.app_view_model.focus_state.set_focus_widget(self.menu_panel)

    def on_menu_selection_changed(self, message: MenuSelectionChanged) -> None:
        self.content_panel.activate(message.tab)

    def on_key(self, event: Key) -> None:
        match event.key:
            case "colon":
                self.command_panel.show()
                event.stop()
                return
            case "tab":
                self.app_view_model.focus_state.focus_next()
                event.stop()
                return
            case _:
                pass

    def _handle_command_globally(self, text: str) -> None:
        match text:
            case "q" | "Q":
                self.app.exit()
        return None

    def on_command_panel_closed(self, message: CommandPanelClosed) -> None:
        match message.reason:
            case "tab":
                self.app_view_model.focus_state.focus_next()
            case _:
                self.app_view_model.focus_state.refocus()

    def on_command_submitted(self, message: CommandSubmitted) -> None:
        text = message.text
        if not text:
            return None

        handled_locally = self.content_panel.send_command_to_active_tab(text)
        if handled_locally:
            return None

        self._handle_command_globally(text)
        return None

    def on_unmount(self) -> None:
        pass


class CliApp(App):
    def __init__(self, container: Container):
        super().__init__()

        self.container = container

    def on_mount(self) -> None:
        self.push_screen(CliAppScreen(self.container))


def build_injected_cli(container: Container):
    app = CliApp(container)

    return app.run
