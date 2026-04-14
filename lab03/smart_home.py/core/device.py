from abc import ABC, abstractmethod
from typing import Dict, Any

class Device(ABC):
    def __init__(self, device_id: str, name: str):
        self.device_id = device_id
        self.name = name

    @abstractmethod
    def get_state(self) -> Dict[str, Any]:
        """Возвращает текущее состояние девайса в виде словаря."""
        pass

    @abstractmethod
    def set_state(self, state: Dict[str, Any]) -> None:
        """Применяет состояние девайса из словаря."""
        pass