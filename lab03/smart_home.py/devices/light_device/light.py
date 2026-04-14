from typing import Dict, Any

from core.device import Device

class Light(Device):
    def __init__(self, device_id: str, name: str):
        super().__init__(device_id, name)
        self.is_on = False
        self.brightness = 100
        self.color = "White"

    def get_state(self) -> Dict[str, Any]:
        """Запоминаем полную конфигурацию лампы."""
        return {
            "is_on": self.is_on,
            "brightness": self.brightness,
            "color": self.color
        }

    def set_state(self, state: Dict[str, Any]) -> None:
        """Восстанавливаем конфигурацию из бэкапа."""
        self.is_on = state.get("is_on", self.is_on)
        self.brightness = state.get("brightness", self.brightness)
        self.color = state.get("color", self.color)
        print(f"[Light:{self.name}] Состояние восстановлено: {self.color}, {self.brightness}%, {'Вкл' if self.is_on else 'Выкл'}")
    
    def turn_on(self):
        self.is_on = True
        print(f"[Light:{self.name}] Лампа включена")

    def turn_off(self):
        self.is_on = False
        print(f"[Light:{self.name}] Лампа выключена")

    def set_brightness(self, level: int):
        self.brightness = level
        print(f"[Light:{self.name}] Яркость изменена на {level}%")

    def set_color(self, new_color: str):
        self.color = new_color
        print(f"[Light:{self.name}] Цвет изменен на {new_color}")