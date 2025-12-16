from partial_injector.partial_container import Container
from prompt_toolkit.widgets import Frame

from .configuration.builder import build_configuration_panel
from .forests.builder import build_forests_panel
from .trails.content_builder import build_trails_panel

from ..cli_state import CliState
from ..models import CliTab


def build_content_panel(state: CliState, container: Container) -> Frame:
    if state.menu_state.selected_item == CliTab.CONFIGURATION:
        return build_configuration_panel(state, container)
    if state.menu_state.selected_item == CliTab.FORESTS:
        return build_forests_panel(state, container)
    if state.menu_state.selected_item == CliTab.TRAILS:
        return build_trails_panel(state, container)

    return build_configuration_panel(state, container)
