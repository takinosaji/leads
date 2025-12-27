import traceback


def get_traceback(error: Exception) -> str:
    return "".join(traceback.format_exception(type(error), error, error.__traceback__)
)