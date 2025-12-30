from partial_injector.partial_container import Container
from textual.app import App, ComposeResult
from textual.screen import Screen
from textual.containers import Horizontal
from textual.events import Key

from leads.cli.textual_cli.app_view_model import AppViewModel
from leads.cli.textual_cli.panels.header_panel.panel import HeaderPanel
from leads.cli.textual_cli.panels.menu_panel.panel import MenuPanel, MenuSelectionChanged, MenuItem
from leads.cli.textual_cli.panels.content_panel.panel import ContentPanel
from leads.cli.textual_cli.panels.command_panel.panel import CommandPanel, CommandSubmitted, CommandPanelClosed
from leads.cli.textual_cli.panels.notification_panel.panel import NotificationPanel
from leads.cli.textual_cli.models import CliTab


class CliAppScreen(Screen):
    def __init__(self, container: Container):
        super().__init__()

        self.container = container

        self.header_panel = HeaderPanel(id="header")
        self.menu_panel = MenuPanel([
            MenuItem("Configuration", CliTab.CONFIGURATION),
            MenuItem("Forests", CliTab.FORESTS),
            MenuItem("Trails", CliTab.TRAILS)
        ])
        self.notification_panel = NotificationPanel(id="notifications")
        self.command_panel = CommandPanel(id="command")

        self.app_view_model = AppViewModel(self.notification_panel.view_model,
                                           self.header_panel.hotkeys_panel.view_model)
        self.content_panel = ContentPanel(self.container, self.app_view_model, id="content")
        self._focus_subscription = None

    def compose(self) -> ComposeResult:
        yield self.header_panel

        with Horizontal(id="body"):
            yield self.menu_panel
            yield self.content_panel

        yield self.command_panel
        yield self.notification_panel

    def _is_command_panel_visible(self) -> bool:
        return not self.command_panel.has_class("hidden")

    def on_mount(self) -> None:
        self.app_view_model.focus_state.build(self.menu_panel, self.content_panel)
        self.app_view_model.focus_state.set_focus_at(self, 0)
        self._focus_subscription = self.app_view_model.focus_state.index_subject.subscribe(
            lambda idx: self.app_view_model.focus_state.set_focus_at(self, idx))

    def on_menu_selection_changed(self, message: MenuSelectionChanged) -> None:
        self.content_panel.activate(message.tab)

    def on_key(self, event: Key) -> None:
        match event.key:
            case "colon":
                self.command_panel.show()
                event.stop()
                return
            case "tab":
                if self._is_command_panel_visible(): # TODO: This is bullshit, refactor later
                    return
                self.app_view_model.focus_state.focus_next(self)
                event.stop()
                return
            case _:
                pass

    def on_command_panel_closed(self, message: CommandPanelClosed) -> None:
        if not self.app_view_model.focus_state.focusable_widgets:
            return
        if message.reason == "tab":
            next_index = (self.app_view_model.focus_state.index + 1) % len(self.app_view_model.focus_state.focusable_widgets)
            self.app_view_model.focus_state.set_focus_at(self, next_index)
        else:
            self.app_view_model.focus_state.set_focus_at(self, self.app_view_model.focus_state.index)

    def _handle_command_globally(self, text: str) -> None:
        if text == "q":
            self.app.exit()
            return

    def on_command_submitted(self, message: CommandSubmitted) -> None:
        text = message.text
        if not text:
            return

        handled_locally = self.content_panel.send_command_to_active_tab(text)
        if handled_locally:
            return

        self._handle_command_globally(text)

    def on_unmount(self) -> None:
        if self._focus_subscription:
            self._focus_subscription.dispose()


class CliApp(App):
    def __init__(self, container: Container):
        super().__init__()
        self.container = container

    def on_mount(self) -> None:
        self.push_screen(CliAppScreen(self.container))


def build_injected_cli(container: Container):
    app = CliApp(container)
    return app.run
