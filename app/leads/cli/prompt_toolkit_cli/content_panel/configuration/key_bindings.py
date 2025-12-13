from leads.cli.prompt_toolkit_cli.cli_state import CliState


def configuration_handle_up_key(state: CliState, event):
    state.configuration_state.selected_content_index = max(state.configuration_state.selected_content_index - 1, 0)


def configuration_handle_down_key(state: CliState, event):
    state.configuration_state.selected_content_index = min(state.configuration_state.selected_content_index + 1,
                                                           state.configuration_state.items_length - 1,
    )

def configuration_handle_any_key(state: CliState, event):
    pass