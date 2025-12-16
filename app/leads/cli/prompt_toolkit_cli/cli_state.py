from typing import Optional

from prompt_toolkit.layout import DynamicContainer

from .models import CliMode, CliTab, CliPanels

CLI_TABS = [
    CliTab.CONFIGURATION,
    CliTab.FORESTS,
    CliTab.TRAILS,
]

class ConfigurationState:
    def __init__(self, selected_content_index, item_length):
        self.selected_content_index = selected_content_index
        self.items_length = item_length

class MenuState:
    def __init__(self, selected_index):
        self.selected_index = selected_index

    @property
    def selected_item(self) -> CliTab:
        return CLI_TABS[self.selected_index]

class CliState:
    def __init__(self):
        self.focusable_controls:dict = {}
        self.focused_control = ...

        self.current_forest_name: str = "Demo Forest"
        self.current_forest_id: str = "forest-001"
        self.current_trail_name: str = "Demo Trail"
        self.current_trail_id: str = "trail-001"

        self.notification_message: Optional[str] = None
        self.notification_is_error: bool = False

        self.mode: CliMode = CliMode.NAVIGATION
        self.command_buffer: str = ""

        self.menu_state: MenuState = MenuState(
            selected_index=0)

        self.configuration_state: ConfigurationState = ConfigurationState(
            selected_content_index=0,
            item_length=0)

        self.edit_buffer: str = ""

    def focus_menu_panel(self, layout):
        layout.focus(self.focusable_controls[CliPanels.MENU])
        self.focused_control = self.focusable_controls[CliPanels.MENU]


    def focus_command_panel(self, layout):
        layout.focus(self.focusable_controls[CliPanels.COMMAND])
        self.focused_control = self.focusable_controls[CliPanels.COMMAND]


    def focus_next_control(self, layout):
        values = list(self.focusable_controls.values())
        current_index = values.index(self.focused_control)
        next_index = (current_index + 1) % len(values)
        next_control = values[next_index]

        layout.focus(next_control)

        #layout.focus(next_control if not isinstance(next_control, DynamicContainer) else next_control.get_children()[0].get_children()[0].content.get_children()[3])


        self.focused_control = next_control

    @property
    def command_line_text(self) -> str:
        if self.mode is CliMode.COMMAND:
            return self.command_buffer
        return ""

    def set_notification(self, message: str, is_error: bool) -> None:
        self.notification_message = message
        self.notification_is_error = is_error

    def clear_notification(self) -> None:
        self.notification_message = None
        self.notification_is_error = False
