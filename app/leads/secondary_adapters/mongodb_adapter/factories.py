from decimal import Decimal
from enum import Enum
from typing import Callable, Any
from pydantic import BaseModel

type MongoDBDtoCreator = Callable[[BaseModel], dict]

def __create_mongo_dto(model: Any):
    def convert_value(value: Any) -> Any:
        if isinstance(value, Decimal):
            return float(value)
        elif isinstance(value, Enum):
            return value.value
        elif isinstance(value, dict):
            return {k: convert_value(v) for k, v in value.items()}
        elif isinstance(value, list):
            return [convert_value(v) for v in value]
        elif isinstance(value, BaseModel):
            value_dic = value.model_dump()
            return {k: convert_value(v) for k, v in value_dic.items()}
        return value

    result = model.model_dump()
    return convert_value(result)
create_mongodb_dto: MongoDBDtoCreator = __create_mongo_dto