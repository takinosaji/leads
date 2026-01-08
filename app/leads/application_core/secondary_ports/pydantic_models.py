import traceback
from enum import Enum
from typing import Callable

from pydantic import BaseModel, ConfigDict, ValidationError
from pydantic.alias_generators import to_camel


type ModelValidationError = dict[str, str]
type ValidationErrorExtractor = Callable[[ValidationError], PydanticValidationError]


model_config = ConfigDict(arbitrary_types_allowed=True,
                          alias_generator=to_camel,
                          populate_by_name=True,
                          validate_assignment=True)

extensible_model_config = ConfigDict(**{**model_config, 'extra': 'allow'})


class ModelsMetadata(str, Enum):
    MASKED = "masked"


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


def is_masked_field(model_instance, model_field: str, metadata_key: str, metadata_value: any) -> bool:
    for field_name, field_info in model_instance.model_fields.items():
        if field_name == model_field and isinstance(field_info.json_schema_extra, dict):
            return field_info.json_schema_extra.get(metadata_key) == metadata_value
    return False
