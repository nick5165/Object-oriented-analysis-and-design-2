from typing import Dict, Any

from core.device import Device

class Socket(Device):
    def __init__(self, device_id: str, name: str):
        super().__init__(device_id, name)
        self.is_on = False

    def get_state(self) -> Dict[str, Any]:
        """Запоминаем, была ли розетка включена."""
        return {
            "is_on": self.is_on
        }

    def set_state(self, state: Dict[str, Any]) -> None:
        """Восстанавливаем состояние (вкл/выкл)."""
        self.is_on = state.get("is_on", self.is_on)
        status_text = "Включена" if self.is_on else "Выключена"
        print(f"[Socket:{self.name}] Состояние восстановлено: {status_text}")
    
    def turn_on(self):
        self.is_on = True
        print(f"[Socket:{self.name}] Питание подано")

    def turn_off(self):
        self.is_on = False
        print(f"[Socket:{self.name}] Питание отключено")