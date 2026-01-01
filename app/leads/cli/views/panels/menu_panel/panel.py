from textual.app import ComposeResult
from textual.containers import Vertical
from textual.widgets import Static
from textual.events import Key
from textual.message import Message

from leads.cli.view_models.hotkeys_view_model import HotkeysViewModel, HotkeyItem
from leads.cli.views.models import CliTab
from leads.cli.view_models.menu_view_model import MenuViewModel


MENU_PANEL_WIDTH = 30


class MenuSelectionChanged(Message):
    def __init__(self, index: int, label: str, tab: CliTab) -> None:
        super().__init__()
        self.index = index
        self.label = label
        self.tab = tab


class MenuItemPanel(Static):
    DEFAULT_CSS = """
    MenuItemPanel {
        width: 1fr;
        height: 3;  
        padding: 0 1;
        content-align: center middle;
        background: #2a2a2a;
        color: #b0b0b0;
        text-style: none;
        text-align: center;
        border: round #3a3a3a;
    }

    MenuPanel:focus MenuItemPanel,
    MenuPanel:focus-within MenuItemPanel {
        background: #000000;
        color: #ffffff;
    }
    
    MenuItemPanel.-selected {
        background: #2a2a2a;
        color: #ffd787;
        text-style: bold;
        border: round #5f87af;
    }

    MenuPanel:focus MenuItemPanel.-selected,
    MenuPanel:focus-within MenuItemPanel.-selected {
        background: #303030;
        color: #ffd787;
    }
    
    """

    def __init__(self, label: str, selected: bool = False) -> None:
        super().__init__(label, classes="menu-item")
        self.set_class(selected, "-selected")


class MenuPanel(Vertical):
    DEFAULT_CSS = f"""
    MenuPanel {{
        width: {MENU_PANEL_WIDTH};
        height: 1fr;
        background: #202020;
        color: #d0d0d0;
        padding: 0 0;
        overflow-y: hidden;
        border: none;
    }}

    MenuPanel:focus,
    MenuPanel:focus-within {{
        background: #000000;
    }}
    """

    def __init__(self,
                 menu_view_model: MenuViewModel,
                 hotkeys_view_model: HotkeysViewModel):
        super().__init__(classes="menu-panel")
        self._view_model = menu_view_model
        self._hotkeys_view_model = hotkeys_view_model
        self.can_focus = True

        self._widgets: list[MenuItemPanel] = []
        self._selected_index_subscription = self._view_model.selected_index_subject.subscribe(self._on_selected_index)

    def compose(self) -> ComposeResult:
        for i, item in enumerate(self._view_model.menu_items):
            widget = MenuItemPanel(item.text, selected=(i == 0))
            self._widgets.append(widget)
            yield widget

    def _on_selected_index(self, new_index: int) -> None:
        for i, w in enumerate(self._widgets):
            w.set_class(i == new_index, "-selected")
        item = self._view_model.menu_items[new_index]
        self.post_message(MenuSelectionChanged(new_index, item.text, item.tab))

    def on_mount(self) -> None:
        self._on_selected_index(self._view_model.selected_index)

    def on_focus(self, event) -> None:
        self._hotkeys_view_model.set_hotkeys([
            HotkeyItem("<Tab>", "Change Focus"),
            HotkeyItem("<↑/↓>", "Navigate Menu"),
            HotkeyItem("<:>", "Enter Command")
        ])
        return None

    def on_key(self, event: Key) -> None:
        current = self._view_model.selected_index
        if event.key == "down":
            self._view_model.set_selected_index((current + 1) % len(self._widgets))
            event.stop()
        elif event.key == "up":
            self._view_model.set_selected_index((current - 1) % len(self._widgets))
            event.stop()

    def on_unmount(self) -> None:
        self._selected_index_subscription.dispose()
