from typing import Optional

from partial_injector.partial_container import Container

from .models import CliMode, CliTab


CLI_TABS = [
    CliTab.CONFIGURATION,
    CliTab.FORESTS,
    CliTab.TRAILS,
]

class ConfigurationState:
    def __init__(self, selected_content_index, item_length):
        self.selected_content_index = selected_content_index
        self.items_length = item_length

class CliState:
    def __init__(self):

        self.selected_index: int = 0
        self.current_forest_name: str = "Demo Forest"
        self.current_forest_id: str = "forest-001"
        self.current_trail_name: str = "Demo Trail"
        self.current_trail_id: str = "trail-001"

        self.notification_message: Optional[str] = None
        self.notification_is_error: bool = False

        self.mode: CliMode = CliMode.NAVIGATION
        self.command_buffer: str = ""

        self.focus_index: int = 0

        self.configuration_state: ConfigurationState = ConfigurationState(
            selected_content_index=0,
            item_length=0)

        self.edit_buffer: str = ""

    @property
    def selected_item(self) -> CliTab:
        return CLI_TABS[self.selected_index]

    @property
    def content_text(self) -> str:
        if self.selected_item is CliTab.CONFIGURATION:
            return "Configuration panel (placeholder)."
        if self.selected_item is CliTab.FORESTS:
            return "Forests panel (placeholder)."
        if self.selected_item is CliTab.TRAILS:
            return "Trails panel (placeholder)."
        return ""

    @property
    def footer_text(self) -> str:
        return (
            f" Forest: {self.current_forest_name} ({self.current_forest_id}) "
            f"| Trail: {self.current_trail_name} ({self.current_trail_id}) "
        )

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
