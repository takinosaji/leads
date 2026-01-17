from typing import Callable
from returns.pipeline import flow
from returns.pointfree import bind
from returns.result import Result

from leads.application_core.forests.models import ForestId
from leads.application_core.forests.services import ForestDeleter


type AllForestDataDeleter = Callable[[ForestId], Result]


def __delete_all_forest_data(dep_delete_forest: ForestDeleter,
                             forest_id: ForestId) -> Result:
    return flow(forest_id,
                dep_delete_forest)
delete_all_forest_data: AllForestDataDeleter = __delete_all_forest_data