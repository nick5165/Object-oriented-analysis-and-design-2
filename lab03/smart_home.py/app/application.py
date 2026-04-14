from core.history import History
from core.command import Command

class Application:
    def __init__(self):
        self.history = History()

    def execute_command(self, command: Command):
        """
        Запуск любой команды. 
        Если команда вернула True (состояние изменилось), 
        она сохраняется в историю.
        """
        if command.execute():
            self.history.push(command)
            print(f"[App] Команда {command.__class__.__name__} выполнена и сохранена.")
        else:
            print(f"[App] Команда {command.__class__.__name__} проигнорирована (состояние не изменилось).")

    def undo(self):
        """
        Отмена последнего действия.
        Достает команду из истории и вызывает её метод undo.
        """
        command = self.history.pop()
        
        if command:
            print(f"[App] Отмена команды: {command.__class__.__name__}")
            command.undo()
        else:
            print("[App] История пуста. Нечего отменять.")