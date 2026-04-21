class AppDirect:
    def __init__(self):
        # Храним историю в виде кортежей: (объект_девайса, словарь_старого_состояния)
        self.history = []

    def execute(self, device, action_callable):
        """
        Выполняет действие напрямую.
        Перед выполнением запоминает состояние девайса.
        Если состояние изменилось, сохраняет его в историю для Undo.
        """
        old_state = device.get_state()
        
        # Выполняем переданную функцию напрямую
        action_callable()
        
        new_state = device.get_state()
        
        # Сравниваем словари. Если состояние изменилось — добавляем в историю
        if old_state != new_state:
            self.history.append((device, old_state))
            print(f"[AppDirect] Действие выполнено. Бэкап для '{device.name}' сохранён.")
        else:
            print(f"[AppDirect] Действие проигнорировано (состояние не изменилось).")

    def undo(self):
        """
        Извлекает последний бэкап и восстанавливает состояние устройства напрямую.
        """
        if not self.history:
            print("[AppDirect] История пуста. Нечего отменять.")
            return

        device, old_state = self.history.pop()
        print(f"[AppDirect] Отмена действия для '{device.name}'...")
        device.set_state(old_state)