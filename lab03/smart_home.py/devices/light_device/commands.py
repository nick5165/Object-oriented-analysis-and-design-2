from core.command import Command
from devices.light_device.light import Light

class TurnOnCommand(Command):
    """Простая команда без дополнительных параметров."""
    def execute(self) -> bool:
        if self.device.is_on:
            return False
        
        self.save_backup()
        self.device.turn_on()
        return True

class ChangeColorCommand(Command):
    """Команда с одним параметром — цветом."""
    def __init__(self, app, device: Light, new_color: str):
        super().__init__(app, device)
        self.new_color = new_color

    def execute(self) -> bool:
        if self.device.color == self.new_color:
            return False
            
        self.save_backup()
        self.device.set_color(self.new_color)
        return True

class RelaxModeCommand(Command):
    """Сложная команда, меняющая сразу несколько параметров."""
    def __init__(self, app, device: Light):
        super().__init__(app, device)
        self.target_brightness = 30
        self.target_color = "Warm Yellow"

    def execute(self) -> bool:
        current = self.device.get_state()
        if current["brightness"] == self.target_brightness and current["color"] == self.target_color:
            return False

        self.save_backup()
        self.device.set_brightness(self.target_brightness)
        self.device.set_color(self.target_color)
        self.device.turn_on()
        return True