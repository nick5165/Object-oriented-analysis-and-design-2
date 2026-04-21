import os
from PyQt6.QtWidgets import QWidget
from PyQt6.QtGui import QPainter, QColor, QPen, QBrush, QPainterPath, QRadialGradient
from PyQt6.QtCore import Qt, QTimer, QUrl
from PyQt6.QtMultimedia import QMediaPlayer, QAudioOutput

class LightWidget(QWidget):
    def __init__(self, device):
        super().__init__()
        self.device = device
        self.setMinimumSize(200, 200)
        self.color_map = {
            "White": QColor(255, 255, 255),
            "Red": QColor(255, 50, 50),
            "Warm Yellow": QColor(255, 200, 100)
        }

    def paintEvent(self, event):
        painter = QPainter(self)
        painter.setRenderHint(QPainter.RenderHint.Antialiasing)
        center_x, center_y = self.width() // 2, self.height() // 2

        if self.device.is_on:
            base_color = self.color_map.get(self.device.color, QColor(255, 255, 255))
            alpha = int((self.device.brightness / 100) * 255)
            
            gradient = QRadialGradient(center_x, center_y, 80)
            gradient.setColorAt(0, QColor(base_color.red(), base_color.green(), base_color.blue(), alpha))
            gradient.setColorAt(1, QColor(base_color.red(), base_color.green(), base_color.blue(), 0))
            
            painter.setBrush(QBrush(gradient))
            painter.setPen(Qt.PenStyle.NoPen)
            painter.drawEllipse(center_x - 100, center_y - 100, 200, 200)
            painter.setBrush(base_color)
        else:
            painter.setBrush(QColor(100, 100, 100))

        painter.setPen(QPen(Qt.GlobalColor.black, 2))
        painter.drawEllipse(center_x - 30, center_y - 30, 60, 60)
        painter.drawRect(center_x - 15, center_y + 30, 30, 20)

class SocketWidget(QWidget):
    def __init__(self, device):
        super().__init__()
        self.device = device
        self.setMinimumSize(200, 200)

    def paintEvent(self, event):
        painter = QPainter(self)
        painter.setRenderHint(QPainter.RenderHint.Antialiasing)
        cx, cy = self.width() // 2, self.height() // 2

        painter.setBrush(QColor(220, 220, 220))
        painter.setPen(QPen(Qt.GlobalColor.darkGray, 3))
        painter.drawRoundedRect(cx - 50, cy - 50, 100, 100, 15, 15)

        painter.setBrush(Qt.GlobalColor.black)
        painter.drawEllipse(cx - 25, cy - 10, 15, 15)
        painter.drawEllipse(cx + 10, cy - 10, 15, 15)

        if self.device.is_on:
            painter.setPen(Qt.PenStyle.NoPen)
            painter.setBrush(QColor(255, 215, 0))
            path = QPainterPath()
            path.moveTo(cx, cy - 30)
            path.lineTo(cx - 15, cy)
            path.lineTo(cx - 2, cy)
            path.lineTo(cx - 5, cy + 30)
            path.lineTo(cx + 15, cy + 5)
            path.lineTo(cx + 2, cy + 5)
            path.closeSubpath()
            painter.drawPath(path)

class VacuumWidget(QWidget):
    def __init__(self, device):
        super().__init__()
        self.device = device
        self.setMinimumSize(250, 200)
        self.x, self.y = 20.0, 20.0
        self.dx, self.dy = 2.0, 2.0
        
        self.timer = QTimer(self)
        self.timer.timeout.connect(self.update_position)
        self.timer.start(30)

    def update_position(self):
        speed = {"Silent": 1.0, "Standard": 3.0, "Turbo": 6.0}.get(self.device.mode, 2.0)
        
        if self.device.is_at_base:
            self.x += (20 - self.x) * 0.1
            self.y += (20 - self.y) * 0.1
        elif self.device.is_cleaning:
            self.x += self.dx * speed
            self.y += self.dy * speed

            if self.x <= 10 or self.x >= self.width() - 40:
                self.dx *= -1
            if self.y <= 10 or self.y >= self.height() - 40:
                self.dy *= -1
                
        self.update()

    def paintEvent(self, event):
        painter = QPainter(self)
        painter.setRenderHint(QPainter.RenderHint.Antialiasing)

        painter.setBrush(QColor(240, 248, 255))
        painter.drawRect(0, 0, self.width(), self.height())

        painter.setBrush(QColor(50, 205, 50))
        painter.drawRect(10, 10, 40, 40)

        painter.setBrush(QColor(105, 105, 105))
        painter.setPen(QPen(Qt.GlobalColor.black, 2))
        painter.drawEllipse(int(self.x), int(self.y), 30, 30)

        if self.device.is_cleaning:
            painter.setBrush(Qt.GlobalColor.cyan)
            painter.drawEllipse(int(self.x) + 10, int(self.y) + 10, 10, 10)

class SpeakerWidget(QWidget):
    def __init__(self, device):
        super().__init__()
        self.device = device
        self.setMinimumSize(200, 200)
        self.wave_radius = 0
        
        self.timer = QTimer(self)
        self.timer.timeout.connect(self.update_waves)
        self.timer.start(50)

    def update_waves(self):
        if self.device.is_playing:
            self.wave_radius += 2
            if self.wave_radius > 60:
                self.wave_radius = 0
            self.update()
        elif self.wave_radius > 0:
            self.wave_radius = 0
            self.update()

    def paintEvent(self, event):
        painter = QPainter(self)
        painter.setRenderHint(QPainter.RenderHint.Antialiasing)
        cx, cy = self.width() // 2, self.height() // 2

        # Корпус колонки
        painter.setBrush(QColor(40, 40, 40))
        painter.setPen(QPen(Qt.GlobalColor.black, 2))
        painter.drawRoundedRect(cx - 40, cy - 70, 80, 140, 15, 15)

        # Динамики
        painter.setBrush(QColor(70, 70, 70))
        painter.drawEllipse(cx - 20, cy - 50, 40, 40)
        painter.drawEllipse(cx - 25, cy + 10, 50, 50)

        # Волны звука
        if self.device.is_playing:
            painter.setPen(QPen(Qt.GlobalColor.cyan, 2))
            painter.setBrush(Qt.BrushStyle.NoBrush)
            
            painter.drawEllipse(cx - 20 - self.wave_radius//2, cy - 50 - self.wave_radius//2, 
                                40 + self.wave_radius, 40 + self.wave_radius)
            painter.drawEllipse(cx - 25 - self.wave_radius//2, cy + 10 - self.wave_radius//2, 
                                50 + self.wave_radius, 50 + self.wave_radius)

class SpeakerUIController:
    def __init__(self, device):
        self.device = device
        self.player = QMediaPlayer()
        self.audio_output = QAudioOutput()
        self.player.setAudioOutput(self.audio_output)
        
        # Храним название текущего загруженного файла, чтобы не сбрасывать его при паузе
        self.current_loaded_track = None

    def sync(self):
        # Синхронизируем громкость
        self.audio_output.setVolume(self.device.volume / 100.0)
        
        if self.device.is_playing and self.device.current_track != "None":
            # Если трек сменился - только тогда загружаем его заново
            if self.current_loaded_track != self.device.current_track:
                file_path = os.path.abspath(f"music/{self.device.current_track}.mp3")
                self.player.setSource(QUrl.fromLocalFile(file_path))
                self.current_loaded_track = self.device.current_track
            
            # Продолжаем воспроизведение с того же места
            if self.player.playbackState() != QMediaPlayer.PlaybackState.PlayingState:
                self.player.play()
        else:
            self.player.pause()