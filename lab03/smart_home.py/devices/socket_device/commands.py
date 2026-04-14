from core.command import Command

class SocketOnCommand(Command):
    """Команда для включения розетки."""
    def execute(self) -> bool:
        if self.device.is_on:
            return False
        
        self.save_backup()
        self.device.turn_on()
        return True

class SocketOffCommand(Command):
    """Команда для выключения розетки."""
    def execute(self) -> bool:
        if not self.device.is_on:
            return False
            
        self.save_backup()
        self.device.turn_off()
        return True

class SocketToggleCommand(Command):
    """Команда, меняющая состояние на противоположное."""
    def execute(self) -> bool:
        self.save_backup()
        
        if self.device.is_on:
            self.device.turn_off()
        else:
            self.device.turn_on()
            
        return True