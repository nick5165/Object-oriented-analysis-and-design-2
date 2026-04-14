from core.device import Device
from typing import Dict, Any

class VacuumCleaner(Device):
    def __init__(self, device_id: str, name: str):
        super().__init__(device_id, name)
        self.is_cleaning = False
        self.mode = "Standard"
        self.is_at_base = True

    def get_state(self) -> Dict[str, Any]:
        """Сохраняем текущий режим и статус уборки."""
        return {
            "is_cleaning": self.is_cleaning,
            "mode": self.mode,
            "is_at_base": self.is_at_base
        }

    def set_state(self, state: Dict[str, Any]) -> None:
        """Восстанавливаем состояние (например, возвращаем на базу или меняем режим)."""
        self.is_cleaning = state.get("is_cleaning", self.is_cleaning)
        self.mode = state.get("mode", self.mode)
        self.is_at_base = state.get("is_at_base", self.is_at_base)
        
        status = "Уборка" if self.is_cleaning else "Ожидание"
        location = "на базе" if self.is_at_base else "в комнате"
        print(f"[Vacuum:{self.name}] Восстановлено: {status}, Режим: {self.mode}, {location}")
    
    def start_cleaning(self):
        self.is_cleaning = True
        self.is_at_base = False
        print(f"[Vacuum:{self.name}] Начал уборку в режиме {self.mode}")

    def stop_cleaning(self):
        self.is_cleaning = False
        print(f"[Vacuum:{self.name}] Уборка приостановлена")

    def go_to_base(self):
        self.is_cleaning = False
        self.is_at_base = True
        print(f"[Vacuum:{self.name}] Возвращаюсь на зарядную станцию")

    def set_work_mode(self, new_mode: str):
        self.mode = new_mode
        print(f"[Vacuum:{self.name}] Режим мощности изменен на: {new_mode}")