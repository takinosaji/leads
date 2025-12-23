from textual.app import ComposeResult
from textual.containers import Horizontal
from textual.message import Message
from textual.widgets import Static, Input
from textual.containers import Container
from textual.events import Key
from typing import Literal


class CommandSubmitted(Message):
    def __init__(self, text: str) -> None:
        super().__init__()
        self.text = text


class CommandPanelClosed(Message):
    def __init__(self, reason: Literal["escape", "submit", "tab", "unknown"]) -> None:
        super().__init__()
        self.reason = reason


class CommandPanel(Container):
    DEFAULT_CSS = """
    CommandPanel {
        dock: bottom;
        height: 3;
        background: #000000;
        padding: 0;
        border: round #268bd2;
        width: 1fr;
        content-align: left middle;
    }

    CommandPanel.hidden {
        display: none;
    }

    CommandPanel > .row {
        layout: horizontal;
        height: 1;
        width: 1fr;
        padding: 0;
    }

    CommandPanel > .row > .label {
        width: 12;
        height: 1;
        content-align: center middle;
        color: #ffaf00;
        text-style: bold;
    }

    CommandPanel > .row > Input {
        width: 1fr;
        height: 1;
        border: none;
        background: #000000;
        color: #ffffff;
        padding: 0;
        content-align: left middle;
    }
    """

    def __init__(self, **kwargs) -> None:
        super().__init__(**kwargs)
        self.input = Input(placeholder="", id="command-input")

    def compose(self) -> ComposeResult:
        with Horizontal(classes="row"):
            yield Static("Command:", classes="label")
            yield self.input

    def on_mount(self) -> None:
        self.add_class("hidden")

    def show(self) -> None:
        self.remove_class("hidden")
        self.input.value = ""
        self.input.focus()

    def hide(self, reason: Literal["escape", "submit", "tab", "unknown"] = "unknown") -> None:
        self.add_class("hidden")
        self.post_message(CommandPanelClosed(reason))

    def on_key(self, event: Key) -> None:
        if event.key == "escape":
            self.hide(reason="escape")
            event.stop()
        elif event.key == "tab":
            self.hide(reason="tab")
            event.stop()

    def on_input_submitted(self, event: Input.Submitted) -> None:
        text = (event.value or "").strip()

        self.post_message(CommandSubmitted(text))
        if text == "q":
            self.app.exit()
        self.hide(reason="submit")
