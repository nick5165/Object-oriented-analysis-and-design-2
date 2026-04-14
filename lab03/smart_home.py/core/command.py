from abc import ABC, abstractmethod

from core.device import Device
class Command(ABC):
    def __init__(self, app, device: Device):
        self.app = app
        self.device = device
        self._backup = None

    def save_backup(self):
        """Сохраняем состояние перед выполнением."""
        self._backup = self.device.get_state()

    def undo(self):
        """Возвращаем старое состояние девайсу."""
        if self._backup is not None:
            self.device.set_state(self._backup)

    @abstractmethod
    def execute(self) -> bool:
        """Основной метод команды."""
        pass