from __future__ import annotations

from dataclasses import dataclass
from textual.app import ComposeResult
from textual.reactive import reactive
from textual.containers import Vertical
from textual.widgets import Static
from textual.events import Key
from textual.message import Message

from leads.cli.textual_cli.models import CliTab

# Match prompt_toolkit menu width
MENU_PANEL_WIDTH = 30


@dataclass(frozen=True)
class MenuItemData:
    text: str
    tab: CliTab


DEFAULT_ITEMS: list[MenuItemData] = [
    MenuItemData("Configuration", CliTab.CONFIGURATION),
    MenuItemData("Forests", CliTab.FORESTS),
    MenuItemData("Trails", CliTab.TRAILS),
]


class MenuSelectionChanged(Message):
    """Message emitted when the selected menu index changes."""

    def __init__(self, index: int, label: str, tab: CliTab) -> None:
        super().__init__()
        self.index = index
        self.label = label
        self.tab = tab


class MenuItem(Static):
    """One menu item with real border and padding controlled by CSS.

    The selected state is represented by the "-selected" class.
    """

    DEFAULT_CSS = """
    MenuItem {
        width: 1fr;
        height: 3;
        border: round #268bd2;
        padding: 0 1;
        background: #202020;
        color: #ffffff;
        content-align: center middle;
        text-align: center;
    }

    MenuItem.-selected {
        background: #303030;
        color: #ffd787;
        text-style: bold;
        border: round #5f87af;
    }

    MenuPanel:focus MenuItem.-selected {
        background: #1e90ff;
        color: #ffffff;
        text-style: bold;
        border: round #87d7ff;
    }
    """

    def __init__(self, label: str, selected: bool = False) -> None:
        super().__init__(label, classes="menu-item")
        self.set_class(selected, "-selected")


class MenuPanel(Vertical):
    """Menu panel that mirrors the prompt_toolkit menu look using CSS borders.

    - Width matches toolkit (30 cols).
    - Background and colors reflect toolkit styles.
    - Selection is styled via CSS; keyboard navigation updates selection.
    """

    DEFAULT_CSS = f"""
    MenuPanel {{
        width: {MENU_PANEL_WIDTH};
        background: #202020;
        color: #d0d0d0;
        padding: 0 0;
        overflow-y: hidden;
        border: none;
    }}
    """

    selected_index: int = reactive(0)

    def __init__(self, items: list[MenuItemData] | None = None) -> None:
        super().__init__(classes="menu-panel")
        self.items: list[MenuItemData] = items or DEFAULT_ITEMS
        self._widgets: list[MenuItem] = []
        self.can_focus = True

    def compose(self) -> ComposeResult:
        for i, item in enumerate(self.items):
            w = MenuItem(item.text, selected=(i == self.selected_index))
            self._widgets.append(w)
            yield w

    def watch_selected_index(self, new_index: int) -> None:
        for i, w in enumerate(self._widgets):
            w.set_class(i == new_index, "-selected")
        item = self.items[new_index]
        self.post_message(MenuSelectionChanged(new_index, item.text, item.tab))

    def on_mount(self) -> None:
        self.watch_selected_index(self.selected_index)

    def on_key(self, event: Key) -> None:
        if event.key in ("down", "j"):
            self.selected_index = (self.selected_index + 1) % len(self._widgets)
            event.stop()
        elif event.key in ("up", "k"):
            self.selected_index = (self.selected_index - 1) % len(self._widgets)
            event.stop()
