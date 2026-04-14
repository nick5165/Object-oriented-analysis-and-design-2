from typing import List, Optional

from core.command import Command

class History:
    def __init__(self):
        self._history: List[Command] = []

    def push(self, command: Command) -> None:
        """Добавляет выполненную команду в историю."""
        self._history.append(command)

    def pop(self) -> Optional[Command]:
        """
        Извлекает и возвращает последнюю команду из истории.
        Если история пуста, возвращает None.
        """
        if not self._history:
            return None
        return self._history.pop()

    def is_empty(self) -> bool:
        """Проверяет, есть ли в истории команды для отмены."""
        return len(self._history) == 0