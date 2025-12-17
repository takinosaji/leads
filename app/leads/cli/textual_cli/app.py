from partial_injector.partial_container import Container
from textual.app import App, ComposeResult
from textual.screen import Screen
from textual.containers import Horizontal
from textual.events import Key

from leads.cli.textual_cli.panels.header_panel.panel import HeaderPanel
from leads.cli.textual_cli.panels.menu_panel.panel import MenuPanel, MenuSelectionChanged, MenuItemData
from leads.cli.textual_cli.panels.content_panel.panel import ContentPanel
from leads.cli.textual_cli.panels.command_panel.panel import CommandPanel, CommandSubmitted
from leads.cli.textual_cli.models import CliTab


class MainScreen(Screen):
    def __init__(self):
        super().__init__()

        self.header_panel = HeaderPanel(id="header")
        self.menu_panel = MenuPanel([
            MenuItemData("Configuration", CliTab.CONFIGURATION),
            MenuItemData("Forests", CliTab.FORESTS),
            MenuItemData("Trails", CliTab.TRAILS),
        ])
        self.content_panel = ContentPanel(id="content")
        self.command_panel = CommandPanel()

    def compose(self) -> ComposeResult:
        yield self.header_panel

        with Horizontal(id="body"):
            yield self.menu_panel
            yield self.content_panel

        yield self.command_panel

    def on_mount(self) -> None:
        if hasattr(self, "menu_panel"):
            self.set_focus(self.menu_panel)

    def on_menu_selection_changed(self, message: MenuSelectionChanged) -> None:
        self.content_panel.set_active(message.tab)

    def on_key(self, event: Key) -> None:
        if event.key == "colon":
            self.command_panel.show()
            event.stop()

    def on_command_submitted(self, message: CommandSubmitted) -> None:
        pass


class CliApp(App):
    def on_mount(self) -> None:
        self.push_screen(MainScreen())


def build_injected_cli(container: Container):
    app = CliApp()
    return app.run
