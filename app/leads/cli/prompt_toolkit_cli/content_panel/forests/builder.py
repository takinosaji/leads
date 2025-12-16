from partial_injector.partial_container import Container
from prompt_toolkit.layout import Window
from prompt_toolkit.layout.controls import FormattedTextControl
from prompt_toolkit.widgets import Frame

from leads.cli.prompt_toolkit_cli.cli_state import CliState


def build_forests_panel(state: CliState, container: Container) -> Frame:
    def get_text():
        lines = [

        ]
        return lines

    control = FormattedTextControl(get_text)
    window = Window(content=control, style="class:content")
    return Frame(body=window, style="class:content")

