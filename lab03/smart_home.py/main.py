from app.application import Application

from devices.light_device.light import Light
from devices.socket_device.socket import Socket
from devices.speaker_device.speaker import Speaker
from devices.vacuum_device.vacuum import VacuumCleaner

from devices.light_device.commands import TurnOnCommand, ChangeColorCommand, RelaxModeCommand
from devices.socket_device.commands import SocketToggleCommand
from devices.speaker_device.commands import ChangeVolumeCommand, PlayTrackCommand
from devices.vacuum_device.commands import StartCleaningCommand, ChangeVacuumModeCommand

def main():
    app = Application()

    lamp = Light("l1", "Лампа в гостиной")
    power_socket = Socket("s1", "Розетка чайника")
    column = Speaker("sp1", "Яндекс Станция")
    robot = VacuumCleaner("v1", "Робот Валли")

    print("--- НАЧАЛО РАБОТЫ ---\n")

    print("Шаг 1: Управление светом")
    app.execute_command(TurnOnCommand(app, lamp))
    app.execute_command(ChangeColorCommand(app, lamp, "Red"))
    app.execute_command(RelaxModeCommand(app, lamp))
    
    app.execute_command(TurnOnCommand(app, lamp))

    print("\nШаг 2: Управление розеткой и колонкой")
    app.execute_command(SocketToggleCommand(app, power_socket))
    app.execute_command(ChangeVolumeCommand(app, column, 70))
    app.execute_command(PlayTrackCommand(app, column, "Show Must Go On"))

    print("\nШаг 3: Управление пылесосом")
    app.execute_command(StartCleaningCommand(app, robot))
    app.execute_command(ChangeVacuumModeCommand(app, robot, "Turbo"))

    print("\n--- ТЕСТИРОВАНИЕ ОТМЕНЫ (UNDO) ---")
    
    print("\nОтменяем режим Турбо:")
    app.undo()

    print("\nОтменяем запуск пылесоса:")
    app.undo()

    print("\nОтменяем смену трека:")
    app.undo()

    print("\nОтменяем режим релакса у лампы:")
    app.undo()

    print("\n--- ИТОГОВОЕ СОСТОЯНИЕ УСТРОЙСТВ ---")
    print(f"Лампа: {lamp.get_state()}")
    print(f"Розетка: {power_socket.get_state()}")
    print(f"Колонка: {column.get_state()}")
    print(f"Пылесос: {robot.get_state()}")

if __name__ == "__main__":
    main()