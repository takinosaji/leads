from prompt_toolkit.layout import Window
from prompt_toolkit.layout.controls import FormattedTextControl

from ..cli_state import CliState


def __build_notification_control(state: CliState) -> FormattedTextControl:
    def get_text():
        message = state.notification_message
        if not message:
            return [("class:notification-empty", "")]

        style = "class:notification-error" if state.notification_is_error else "class:notification-success"
        return [(style, message)]

    return FormattedTextControl(get_text)


def build_notification_panel(state: CliState) -> Window:
    return Window(
        height=1,
        content=__build_notification_control(state),
        style="class:notification-bar",
    )

