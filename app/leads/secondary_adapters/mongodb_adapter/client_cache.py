from __future__ import annotations

from typing import Optional

from pymongo import MongoClient

from leads.cli.configuration.models import CliConfigurationCache
from leads.secondary_adapters.mongodb_adapter.configuration import MongoDbStorageConfiguration


class MongoDbClientCache:
    def __init__(self,
                 configuration_cache: CliConfigurationCache,
    ) -> None:
        self._configuration_cache = configuration_cache
        self._client: Optional[MongoClient] = None
        self._last_connection_string: Optional[str] = None

    def _get_current_mongodb_config(self) -> Optional[MongoDbStorageConfiguration]:
        configuration = self._configuration_cache.configuration
        if configuration is None:
            raise RuntimeError("CLI configuration has not been loaded into CliConfigurationCache.")
        return configuration.mongodb_storage_configuration

    def _ensure_client_up_to_date(self) -> Optional[MongoClient]:
        mongo_config = self._get_current_mongodb_config()

        current_connection_string: Optional[str]
        current_database_name: Optional[str]
        if mongo_config is None:
            current_connection_string = None
            current_database_name = None
        else:
            current_connection_string = mongo_config.connection_string
            current_database_name = mongo_config.database_name

        config_changed = (
            current_connection_string != self._last_connection_string
        )

        if config_changed:
            if self._client is not None:
                self._client.close()
                self._client = None

            self._last_connection_string = current_connection_string

            if not current_connection_string:
                return None

            self._client = MongoClient(current_connection_string)

        return self._client

    @property
    def client(self) -> Optional[MongoClient]:
        return self._ensure_client_up_to_date()

    def close(self) -> None:
        if self._client is not None:
            self._client.close()
            self._client = None

        self._last_connection_string = None
        self._last_database_name = None
