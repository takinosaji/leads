from partial_injector.partial_container import Container, FromContainer

from leads.secondary_adapters.sqlite_adapter.configuration import SQLiteStorageConfiguration


def register_dependencies(container: Container) -> None:
    container.register_instance(FromContainer(CliConfiguration, lambda conf: conf.order_storage_configuration),
                                key=SQLiteStorageConfiguration,
                                condition=lambda conf: type(conf) == SQLiteStorageConfiguration,
                                condition_args=[FromContainer(StorageConfiguration)],
                                condition_ignore_not_satisfied=True)

    # container.register_instance(upsert_order,
    #                             key=OrderStorageUpserter,
    #                             condition=lambda conf: type(conf) == OrderStorageInMemoryConfiguration,
    #                             condition_args=[FromContainer(OrderStorageConfiguration)])
