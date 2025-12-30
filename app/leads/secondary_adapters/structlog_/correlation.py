import structlog
from dot_thrns.hrns.cli.application_core.secondary_ports.correlation import CorrelationContextCleaner, \
    CorrelationContextBinder, CorrelationContext

def ___clear_correlation_contextvars() -> None:
    structlog.contextvars.clear_contextvars()
    return None
clear_correlation_contextvars: CorrelationContextCleaner = __clear_correlation_contextvars

def ___bind_correlation_contextvars(correlation_context: CorrelationContext) -> None:
    structlog.contextvars.bind_contextvars(**correlation_context)
    return None
bind_correlation_contextvars: CorrelationContextBinder = __bind_correlation_contextvars