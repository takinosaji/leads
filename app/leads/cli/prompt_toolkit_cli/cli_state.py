from .models import CliMode


CLI_TABS = [
    "Configuration",
    "Forests",
    "Trails"]


class CliState:
    selected_index: int = 0
    current_forest_name: str = "Demo Forest"
    current_forest_id: str = "forest-001"
    current_trail_name: str = "Demo Trail"
    current_trail_id: str = "trail-001"

    mode: CliMode = CliMode.NAVIGATION
    command_buffer: str = ""

    @property
    def selected_item(self):
        return CLI_TABS[self.selected_index]

    @property
    def content_text(self):
        if self.selected_item == "Configuration":
            return "Configuration panel (placeholder)."
        if self.selected_item == "Forests":
            return "Forests panel (placeholder)."
        if self.selected_item == "Trails":
            return "Trails panel (placeholder)."
        return ""

    @property
    def footer_text(self):
        return (
            f" Forest: {self.current_forest_name} ({self.current_forest_id}) "
            f"| Trail: {self.current_trail_name} ({self.current_trail_id}) "
        )

    @property
    def command_line_text(self):
        if self.mode is CliMode.EDITING:
            return self.command_buffer
        return ""
