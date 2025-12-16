from partial_injector.partial_container import Container
from prompt_toolkit.layout import Window
from prompt_toolkit.layout.controls import FormattedTextControl
from prompt_toolkit.widgets import Frame

from leads.cli.configuration.models import CliConfiguration
from leads.cli.prompt_toolkit_cli.cli_state import CliState
from leads.cli.prompt_toolkit_cli.models import CliMode, CliPanels

KEY_WIDTH = 28
VALUE_WIDTH = 30
SEPARATOR = " │ "


def __build_configuration_control(state: CliState, container: Container) -> FormattedTextControl:
    def get_text():
        rows: list[tuple[str, str]] = __get_configuration_rows(container)
        state.configuration_state.items_length = len(rows)

        lines: list[tuple[str, str]] = []

        header_key = "Key".ljust(KEY_WIDTH)
        header_val = "Value".ljust(VALUE_WIDTH)
        header_line = f" {header_key}{SEPARATOR}{header_val} \n"
        separator_line = " " + "─" * (KEY_WIDTH + len(SEPARATOR) + VALUE_WIDTH) + " \n"

        lines.append(("class:content-text", header_line))
        lines.append(("class:content-text", separator_line))

        for index, (key, value) in enumerate(rows):
            is_selected = index == state.configuration_state.selected_content_index
            if is_selected:
                style = "class:content-selected-active" if state.menu_state.selected_index == 1 else "class:content-selected-inactive"
            else:
                style = "class:content-text"

            display_value = value
            if state.mode is CliMode.COMMAND and is_selected:
                display_value = state.edit_buffer

            padded_key = key.ljust(KEY_WIDTH)
            padded_val = str(display_value).ljust(VALUE_WIDTH)
            row_line = f" {padded_key}{SEPARATOR}{padded_val} \n"

            lines.append((style, row_line))
        return lines

    return FormattedTextControl(get_text)


def __get_configuration_rows(container: Container):
    configuration = container.resolve(CliConfiguration)

    return [(
        "context.active_forest",
        str(configuration.context_configuration.active_forest),
    ), (
        "runtime.min_log_level",
        str(configuration.runtime_configuration.min_log_level),
    )]


def build_configuration_panel(state: CliState, container: Container) -> Frame:
    control = __build_configuration_control(state, container)
    window = Window(content=control, style="class:content")
    return Frame(body=window, style="class:content")
