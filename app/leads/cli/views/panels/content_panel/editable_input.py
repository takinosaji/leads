from textual import events
from textual.widgets import Input


class EditableInput(Input):
    def __init__(self, editing_state, **kwargs):
        super().__init__(**kwargs)

        self.editing_state = editing_state

    def on_mount(self) -> None:
        self.focus()
        self.cursor_position = len(self.value)

    def on_key(self, event: events.Key) -> None:
        match event.key:
            case "escape":
                self.editing_state.end()
                event.stop()
            case "tab":
                self.editing_state.end()
            case "enter":
                self.editing_state.apply(self.value)
                event.stop()
