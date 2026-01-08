from typing import Callable
from partial_injector.partial_container import Container
from returns.result import safe
from spinq import dicts
from textual.markup import escape
from rx.subject import BehaviorSubject

from leads.cli.configuration.factory import CliConfigurationLoader, CliConfigurationSaver
from leads.cli.configuration.models import CliConfiguration, ContextConfiguration, RuntimeConfiguration
from leads.cli.views.models import FlatConfiguration
from leads.cli.view_models.notification_view_model import NotificationViewModel
from leads.secondary_adapters.mongodb_adapter.configuration import MongoDbStorageConfiguration
from leads.secondary_adapters.sqlite_adapter.configuration import SQLiteStorageConfiguration


class InitializedFocusState:
    def __init__(self, index, total_rows: int) -> None:
        self.total_rows = total_rows
        self.index = index

    def move_next(self) -> None:
        if self.total_rows:
            self.index = (self.index + 1) % self.total_rows

    def move_prev(self) -> None:
        if self.total_rows:
            self.index = (self.index - 1) % self.total_rows


class EditingState:
    def __init__(self,
                 view_model: ConfigurationViewModel,
                 changed: Callable[[], None]):
        self._view_model: ConfigurationViewModel = view_model
        self._changed = changed
        self.row_index: int | None = None
        self.key: str | None = None

    def start(self, idx: int, key: str, value: str):
        self.row_index = idx
        self.key = key
        self.value = value
        self._changed()

    def end(self):
        self.row_index = None
        self.key = None
        self.value = None
        self._changed()

    def apply(self, value):
        key, _ = dicts.get_key_value_by_index_(self._view_model.data.__dict__, self.row_index)
        setattr(self._view_model.data, key, value)
        self.end()


class ConfigurationViewModel:
    def __init__(self,
                 container: Container,
                 notification_view_model: NotificationViewModel):
        self.__container: Container = container
        self.notification_view_model: NotificationViewModel = notification_view_model
        self.data: FlatConfiguration | None = None
        self.focus_state: InitializedFocusState | None = None
        self.edit_state: EditingState = EditingState(self, lambda: self._subject.on_next(None))
        self._subject = BehaviorSubject(None)

    def subscribe(self, observer):
        return self._subject.subscribe(observer)

    def clear_state(self) -> None:
        self.data = None
        self.focus_state = None

    @safe
    def load_configuration(self):
        @safe
        def map_to_flat(conf: CliConfiguration):
            return FlatConfiguration(
                min_log_level=conf.runtime_configuration.min_log_level,
                active_forest=conf.context_configuration.active_forest or "",
                mongodb_storage_connection_string=(conf.mongodb_storage_configuration.connection_string
                                                   if conf.mongodb_storage_configuration.connection_string else ""),
                mongodb_storage_database_name=(conf.mongodb_storage_configuration.database_name
                                               if conf.mongodb_storage_configuration.database_name else ""),
                sqlite_storage_connection_string=(conf.sqlite_storage_configuration.connection_string
                                                  if conf.sqlite_storage_configuration.connection_string else ""))

        if self.data is None:
            self.data = (
                self.__container.resolve(CliConfigurationLoader)()
                .bind(map_to_flat)
                .unwrap()
            )
        return self.data

    @safe
    def save_configuration(self):
        self.notification_view_model.clear_notifications()
        try:
            cli_configuration = CliConfiguration(
                context_configuration=ContextConfiguration(
                    active_forest=self.data.active_forest if self.data.active_forest else None
                ),
                runtime_configuration=RuntimeConfiguration(
                    min_log_level=self.data.min_log_level
                ),
                sqlite_storage_configuration=SQLiteStorageConfiguration(
                    connection_string=(self.data.sqlite_storage_connection_string
                                       if self.data.sqlite_storage_connection_string else None)
                ),
                mongodb_storage_configuration=MongoDbStorageConfiguration(
                    connection_string=(self.data.mongodb_storage_connection_string
                                       if self.data.mongodb_storage_connection_string else None),
                    database_name=(self.data.mongodb_storage_database_name
                                   if self.data.mongodb_storage_database_name else None)
                )
            )
            self.__container.resolve(CliConfigurationSaver)(cli_configuration)
            self.notification_view_model.add_notification("Configuration saved successfully.")
            self.data = None
            self._subject.on_next(None)
        except Exception as error:
            self.notification_view_model.add_notification(escape(str(error)), True)
