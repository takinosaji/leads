from partial_injector.partial_container import Container
from textual.app import App, ComposeResult
from textual.screen import Screen
from textual.containers import Horizontal
from textual.events import Key
from textual.widget import Widget

from leads.cli.textual_cli.panels.header_panel.panel import HeaderPanel
from leads.cli.textual_cli.panels.menu_panel.panel import MenuPanel, MenuSelectionChanged, MenuItemData
from leads.cli.textual_cli.panels.content_panel.panel import ContentPanel
from leads.cli.textual_cli.panels.command_panel.panel import CommandPanel, CommandSubmitted, CommandPanelClosed
from leads.cli.textual_cli.models import CliTab


class AppFocusState:
    def __init__(self) -> None:
        self.focusable_widgets: list[Widget] = []
        self.index: int = 0
        self.last_main_index: int = 0

    def build(self, *widgets: Widget) -> None:
        self.focusable_widgets = [w for w in widgets if isinstance(w, Widget)]
        self.index = min(self.index, max(0, len(self.focusable_widgets) - 1))

    def set_focus_at(self, screen: Screen, index: int) -> None:
        if not self.focusable_widgets:
            return
        index = index % len(self.focusable_widgets)
        screen.set_focus(self.focusable_widgets[index])
        self.index = index

    def focus_next(self, screen: Screen) -> None:
        if not self.focusable_widgets:
            return
        self.set_focus_at(screen, (self.index + 1) % len(self.focusable_widgets))

    def sync_from_screen(self, screen: Screen) -> None:
        current = screen.focused
        if current in self.focusable_widgets:
            self.index = self.focusable_widgets.index(current)


class CliAppScreen(Screen):
    def __init__(self, container: Container):
        super().__init__()

        self.container = container

        self.header_panel = HeaderPanel(id="header")
        self.menu_panel = MenuPanel([
            MenuItemData("Configuration", CliTab.CONFIGURATION),
            MenuItemData("Forests", CliTab.FORESTS),
            MenuItemData("Trails", CliTab.TRAILS),
        ])
        self.content_panel = ContentPanel(container, id="content")
        self.command_panel = CommandPanel()

        self.focus_state = AppFocusState()

    def compose(self) -> ComposeResult:
        yield self.header_panel

        with Horizontal(id="body"):
            yield self.menu_panel
            yield self.content_panel

        yield self.command_panel

    def _is_command_panel_visible(self) -> bool:
        return not self.command_panel.has_class("hidden")

    def on_mount(self) -> None:
        self.focus_state.build(self.menu_panel, self.content_panel)
        self.focus_state.set_focus_at(self, 0)

    def on_menu_selection_changed(self, message: MenuSelectionChanged) -> None:
        self.content_panel.set_active(message.tab)

    def on_key(self, event: Key) -> None:
        match event.key:
            case "colon":
                self.focus_state.sync_from_screen(self)
                self.focus_state.last_main_index = self.focus_state.index
                self.command_panel.show()
                event.stop()
                return
            case "tab":
                if self._is_command_panel_visible():
                    return
                self.focus_state.focus_next(self)
                event.stop()
                return
            case _:
                pass

    def on_command_panel_closed(self, message: CommandPanelClosed) -> None:
        if not self.focus_state.focusable_widgets:
            return
        if message.reason == "tab":
            next_index = (self.focus_state.last_main_index + 1) % len(self.focus_state.focusable_widgets)
            self.focus_state.set_focus_at(self, next_index)
        else:
            self.focus_state.set_focus_at(self, self.focus_state.last_main_index)

    def on_command_submitted(self, message: CommandSubmitted) -> None:
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
