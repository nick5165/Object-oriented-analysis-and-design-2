import sys
from PyQt6.QtWidgets import QApplication

# Импортируем наше главное окно из папки gui
from gui.main_window import MainWindow

def main():
    app = QApplication(sys.argv)
    
    # Задаем базовые стили для красоты интерфейса
    app.setStyleSheet("""
        QGroupBox {
            font-weight: bold;
            border: 2px solid gray;
            border-radius: 5px;
            margin-top: 1ex;
        }
        QGroupBox::title {
            subcontrol-origin: margin;
            left: 10px;
            padding: 0 3px 0 3px;
        }
        QPushButton {
            background-color: #f0f0f0;
            border: 1px solid #ccc;
            padding: 8px;
            border-radius: 4px;
            font-size: 12px;
        }
        QPushButton:hover {
            background-color: #e0e0e0;
        }
        QPushButton:pressed {
            background-color: #d0d0d0;
        }
    """)

    window = MainWindow()
    window.show()
    
    # Запуск бесконечного цикла обработки событий приложения
    sys.exit(app.exec())

if __name__ == "__main__":
    main()