from core.command import Command
from devices.vacuum_device.vacuum import VacuumCleaner

class StartCleaningCommand(Command):
    """Команда начала уборки."""
    def execute(self) -> bool:
        if self.device.is_cleaning:
            return False
        
        self.save_backup()
        self.device.start_cleaning()
        return True

class ReturnToBaseCommand(Command):
    """Команда возврата на зарядную станцию."""
    def execute(self) -> bool:
        if self.device.is_at_base:
            return False
            
        self.save_backup()
        self.device.go_to_base()
        return True

class ChangeVacuumModeCommand(Command):
    """Команда изменения режима мощности (Silent, Standard, Turbo)."""
    def __init__(self, app, device: VacuumCleaner, new_mode: str):
        super().__init__(app, device)
        self.new_mode = new_mode

    def execute(self) -> bool:
        if self.device.mode == self.new_mode:
            return False
            
        self.save_backup()
        self.device.set_work_mode(self.new_mode)
        return True