def menu_handle_up_key(state, event):
    state.menu_state.selected_index = (state.menu_state.selected_index - 1) % len(CLI_TABS)
    state.clear_notification()
    event.app.invalidate()