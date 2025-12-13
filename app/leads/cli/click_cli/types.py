from typing import Callable, Container

from click import Command

type CliCommandFactory = Callable[[Container], Command]