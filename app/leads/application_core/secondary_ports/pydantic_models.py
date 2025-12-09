import traceback
from typing import Callable

from pydantic import BaseModel, ConfigDict, ValidationError
from pydantic.alias_generators import to_camel, to_pascal


type ModelValidationError = dict[str, str]
type ValidationErrorExtractor = Callable[[ValidationError], PydanticValidationError]


model_config = ConfigDict(arbitrary_types_allowed=True,
                          alias_generator=to_camel,
                          populate_by_name=True)

pascal_model_config = ConfigDict(**{**model_config, 'alias_generator': to_pascal})
extensible_model_config = ConfigDict(**{**model_config, 'extra': 'allow'})
extensible_pascal_model_config = ConfigDict(**{**pascal_model_config, 'extra': 'allow'})


class PydanticValidationError(BaseModel):
    model_config = model_config

    errors: list[ModelValidationError]
    stack_trace: str


def model_dump(model: BaseModel) -> dict:
    return model.model_dump(exclude_none=True)


def model_dump_by_alias(model: BaseModel) -> dict:
    return model.model_dump(by_alias=True, exclude_none=True)


def __extract_validation_error(validation_error: ValidationError) -> PydanticValidationError:
    stack_trace = ''.join(traceback.format_tb(validation_error.__traceback__))

    validation_errors = []
    for error in validation_error.errors():
        validation_errors.append({
            "loc": " -> ".join(str(loc) for loc in error['loc']),
            "msg": error['msg']
        })

    return PydanticValidationError(stack_trace=stack_trace, errors=validation_errors)


extract_validation_error: ValidationErrorExtractor = __extract_validation_error
