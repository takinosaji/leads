from __future__ import annotations

from typing import Optional, Mapping, Any

from pymongo import MongoClient
from pymongo.synchronous.database import Database

from leads.secondary_adapters.mongodb_adapter.configuration import MongoDbStorageConfiguration


class MongoDbClientCache:
    def __init__(self,
                 configuration: MongoDbStorageConfiguration,
    ) -> None:
        self._configuration = configuration
        self._client: Optional[MongoClient] = None
        self._last_connection_string: Optional[str] = None
        self._last_database_name: Optional[str] = None

    def _ensure_client_up_to_date(self) -> Optional[MongoClient]:
        mongo_config = self._configuration

        current_connection_string: Optional[str]
        current_database_name: Optional[str]
        if mongo_config is None:
            current_connection_string = None
            current_database_name = None
        else:
            current_connection_string = mongo_config.connection_string
            current_database_name = mongo_config.database_name

        config_changed = (
            current_connection_string != self._last_connection_string and
            current_database_name != self._last_database_name
        )

        if config_changed:
            if self._client is not None:
                self._client.close()
                self._client = None

            self._last_connection_string = current_connection_string
            self._last_database_name = current_database_name

            if not current_connection_string:
                return None

            # Timeouts are in milliseconds; tweak these defaults as needed
            self._client = MongoClient(
                current_connection_string,
                serverSelectionTimeoutMS=2000,
                socketTimeoutMS=2000,
                connectTimeoutMS=2000,
            )

        return self._client

    @property
    def client(self) -> Optional[MongoClient]:
        return self._ensure_client_up_to_date()

    @property
    def database(self) -> Database[Mapping[str, Any] | Any]:
        return self.client[self._last_database_name]

    def close(self) -> None:
        if self._client is not None:
            self._client.close()
            self._client = None

        self._last_connection_string = None
        self._last_database_name = None
