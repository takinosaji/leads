from typing import Callable

type CorrelationKey = str
type CorrelationId = str
type CorrelationContext = dict[CorrelationKey, CorrelationId]
type CorrelationContextBinder = Callable[[CorrelationContext], None]
type CorrelationContextCleaner = Callable[[], None]