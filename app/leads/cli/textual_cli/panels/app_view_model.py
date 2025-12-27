from leads.cli.textual_cli.panels.notification_panel.view_model import NotificationViewModel


class AppViewModel:
    def __init__(self, notification_view_model: NotificationViewModel):
        self.notification_view_model: NotificationViewModel = notification_view_model