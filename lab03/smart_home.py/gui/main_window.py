import os
from PyQt6.QtWidgets import (QMainWindow, QWidget, QVBoxLayout, QHBoxLayout, 
                             QPushButton, QLabel, QGroupBox, QComboBox, QGridLayout, QFrame)
from PyQt6.QtCore import Qt
from PyQt6.QtGui import QFont

from app.application import Application
from devices.light_device.commands import LightToggleCommand, ChangeColorCommand, RelaxModeCommand
from devices.light_device.light import Light
from devices.socket_device.commands import SocketToggleCommand
from devices.socket_device.socket import Socket
from devices.speaker_device.commands import PlayTrackCommand, MediaToggleCommand, ChangeVolumeCommand, SpeakerNextCommand, SpeakerPrevCommand
from devices.speaker_device.speaker import Speaker
from devices.vacuum_device.commands import StartCleaningCommand, ReturnToBaseCommand, ChangeVacuumModeCommand
from devices.vacuum_device.vacuum import VacuumCleaner

from gui.widgets import LightWidget, SocketWidget, VacuumWidget, SpeakerWidget, SpeakerUIController

class MainWindow(QMainWindow):
    def __init__(self):
        super().__init__()
        self.setWindowTitle("Умный дом: Комната и Пульт")
        self.resize(1100, 700)

        self.app = Application()
        
        self.lamp = Light("l1", "Лампа")
        self.socket = Socket("s1", "Розетка")
        self.speaker = Speaker("sp1", "Колонка")
        self.vacuum = VacuumCleaner("v1", "Пылесос")

        tracks = self.get_music_tracks()
        self.speaker.set_playlist(tracks)

        self.speaker_controller = SpeakerUIController(self.speaker)

        self.init_ui(tracks)
        self.sync_ui()

    def get_music_tracks(self):
        if not os.path.exists("music"):
            os.makedirs("music")
        tracks = [f[:-4] for f in os.listdir("music") if f.endswith(".mp3")]
        return tracks if tracks else ["Нет треков"]

    def init_ui(self, tracks):
        main_widget = QWidget()
        main_layout = QHBoxLayout()

        # === ЛЕВАЯ ЧАСТЬ ===
        room_layout = QGridLayout()
        
        light_box = QGroupBox("Умная лампа")
        l_lay = QVBoxLayout()
        self.light_widget = LightWidget(self.lamp)
        l_lay.addWidget(self.light_widget)
        light_box.setLayout(l_lay)

        socket_box = QGroupBox("Умная розетка")
        s_lay = QVBoxLayout()
        self.socket_widget = SocketWidget(self.socket)
        s_lay.addWidget(self.socket_widget)
        socket_box.setLayout(s_lay)

        vacuum_box = QGroupBox("Робот-пылесос")
        v_lay = QVBoxLayout()
        self.vacuum_widget = VacuumWidget(self.vacuum)
        v_lay.addWidget(self.vacuum_widget)
        vacuum_box.setLayout(v_lay)

        speaker_box = QGroupBox("Умная колонка")
        sp_lay = QVBoxLayout()
        self.speaker_widget = SpeakerWidget(self.speaker)
        self.lbl_speaker_status = QLabel("...")
        self.lbl_speaker_status.setAlignment(Qt.AlignmentFlag.AlignCenter)
        sp_lay.addWidget(self.speaker_widget)
        sp_lay.addWidget(self.lbl_speaker_status)
        speaker_box.setLayout(sp_lay)

        room_layout.addWidget(light_box, 0, 0)
        room_layout.addWidget(socket_box, 0, 1)
        room_layout.addWidget(vacuum_box, 1, 0)
        room_layout.addWidget(speaker_box, 1, 1)

        main_layout.addLayout(room_layout, stretch=3)

        # === ПРАВАЯ ЧАСТЬ: ПУЛЬТ ===
        remote_frame = QFrame()
        remote_frame.setStyleSheet("""
            QFrame {
                background-color: #2c3e50;
                border-radius: 25px;
                border: 3px solid #1a252f;
            }
            QLabel { color: white; font-weight: bold; }
            QLabel#screen_label { color: #2ecc71; margin-top: 10px; }
            QComboBox { 
                padding: 5px; border-radius: 5px; 
                background-color: white; color: black;
            }
            QComboBox#screen {
                background-color: #a3c2c2; color: #000;
                font-family: 'Courier New', monospace; font-size: 14px;
                font-weight: bold; border: 2px solid #7f8c8d; padding: 10px;
            }
            QPushButton {
                background-color: #ecf0f1; color: #2c3e50;
                border-radius: 10px; padding: 10px; font-weight: bold; margin: 2px;
            }
            QPushButton:hover { background-color: #bdc3c7; }
            QPushButton:pressed { background-color: #95a5a6; }
            QPushButton#power { background-color: #e74c3c; color: white; }
            QPushButton#power:hover { background-color: #c0392b; }
            QPushButton#undo { background-color: #f39c12; color: white; }
        """)
        remote_layout = QVBoxLayout()

        lbl_remote = QLabel("ПУЛЬТ")
        lbl_remote.setAlignment(Qt.AlignmentFlag.AlignCenter)
        font = QFont()
        font.setPointSize(14)
        lbl_remote.setFont(font)
        
        self.combo_target = QComboBox()
        self.combo_target.addItems(["Лампа", "Розетка", "Колонка", "Пылесос"])

        btn_power = QPushButton("Вкл / Выкл")
        btn_power.setObjectName("power")
        btn_power.clicked.connect(self.on_power_pressed)

        btn_mode = QPushButton("Режим (Relax / Turbo)")
        btn_mode.clicked.connect(self.on_mode_pressed)

        btn_color = QPushButton("Цвет (Красный)")
        btn_color.clicked.connect(self.on_color_pressed)

        btn_base = QPushButton("На базу (Пылесос)")
        btn_base.clicked.connect(self.on_base_pressed)

        # "Экранчик" на пульте
        lbl_screen = QLabel("МЕДИАПЛЕЕР:")
        lbl_screen.setObjectName("screen_label")
        lbl_screen.setAlignment(Qt.AlignmentFlag.AlignCenter)
        
        self.combo_tracks = QComboBox()
        self.combo_tracks.setObjectName("screen")
        self.combo_tracks.addItems(tracks)

        # Переименованная кнопка
        btn_play = QPushButton("▶ Включить трек")
        btn_play.clicked.connect(self.on_play_pressed)
        
        btn_pause = QPushButton("⏯ Пауза")
        btn_pause.clicked.connect(self.on_pause_pressed)

        btn_prev = QPushButton("⏮ Prev")
        btn_prev.clicked.connect(self.on_prev_pressed)
        
        btn_next = QPushButton("Next ⏭")
        btn_next.clicked.connect(self.on_next_pressed)

        btn_vol_up = QPushButton("Vol +")
        btn_vol_up.clicked.connect(lambda: self.on_vol_pressed(True))

        btn_vol_down = QPushButton("Vol -")
        btn_vol_down.clicked.connect(lambda: self.on_vol_pressed(False))

        btn_undo = QPushButton("Отменить (UNDO)")
        btn_undo.setObjectName("undo")
        btn_undo.clicked.connect(self.on_undo_pressed)

        # Сборка пульта
        remote_layout.addWidget(lbl_remote)
        remote_layout.addWidget(QLabel("Устройство:"))
        remote_layout.addWidget(self.combo_target)
        remote_layout.addSpacing(10)
        
        remote_layout.addWidget(btn_power)
        remote_layout.addWidget(btn_mode)
        remote_layout.addWidget(btn_color)
        remote_layout.addWidget(btn_base)
        
        remote_layout.addSpacing(15)
        
        remote_layout.addWidget(lbl_screen)
        remote_layout.addWidget(self.combo_tracks)
        
        h_media1 = QHBoxLayout()
        h_media1.addWidget(btn_play)
        h_media1.addWidget(btn_pause)
        remote_layout.addLayout(h_media1)

        h_media2 = QHBoxLayout()
        h_media2.addWidget(btn_prev)
        h_media2.addWidget(btn_next)
        remote_layout.addLayout(h_media2)
        
        h_vol = QHBoxLayout()
        h_vol.addWidget(btn_vol_down)
        h_vol.addWidget(btn_vol_up)
        remote_layout.addLayout(h_vol)
        
        remote_layout.addStretch()
        remote_layout.addWidget(btn_undo)

        remote_frame.setLayout(remote_layout)
        remote_frame.setFixedWidth(300)

        main_layout.addWidget(remote_frame, stretch=1)
        main_widget.setLayout(main_layout)
        self.setCentralWidget(main_widget)

    # === ЛОГИКА ПУЛЬТА ===
    def current_target(self):
        return self.combo_target.currentText()

    def execute(self, command):
        self.app.execute_command(command)
        self.sync_ui()

    def on_power_pressed(self):
        target = self.current_target()
        if target == "Лампа": self.execute(LightToggleCommand(self.app, self.lamp))
        elif target == "Розетка": self.execute(SocketToggleCommand(self.app, self.socket))
        elif target == "Пылесос": 
            if self.vacuum.is_cleaning:
                self.execute(ReturnToBaseCommand(self.app, self.vacuum))
            else:
                self.execute(StartCleaningCommand(self.app, self.vacuum))

    def on_mode_pressed(self):
        target = self.current_target()
        if target == "Лампа": self.execute(RelaxModeCommand(self.app, self.lamp))
        elif target == "Пылесос": self.execute(ChangeVacuumModeCommand(self.app, self.vacuum, "Turbo"))

    def on_color_pressed(self):
        if self.current_target() == "Лампа":
            self.execute(ChangeColorCommand(self.app, self.lamp, "Red"))

    def on_play_pressed(self):
        if self.current_target() == "Колонка":
            track = self.combo_tracks.currentText()
            if track != "Нет треков":
                self.execute(PlayTrackCommand(self.app, self.speaker, track))

    def on_pause_pressed(self):
        if self.current_target() == "Колонка":
            self.execute(MediaToggleCommand(self.app, self.speaker))

    def on_next_pressed(self):
        if self.current_target() == "Колонка":
            self.execute(SpeakerNextCommand(self.app, self.speaker))

    def on_prev_pressed(self):
        if self.current_target() == "Колонка":
            self.execute(SpeakerPrevCommand(self.app, self.speaker))

    def on_vol_pressed(self, is_up):
        if self.current_target() == "Колонка":
            step = 10 if is_up else -10
            new_vol = max(0, min(100, self.speaker.volume + step))
            self.execute(ChangeVolumeCommand(self.app, self.speaker, new_vol))

    def on_base_pressed(self):
        if self.current_target() == "Пылесос":
            self.execute(ReturnToBaseCommand(self.app, self.vacuum))

    def on_undo_pressed(self):
        self.app.undo()
        self.sync_ui()

    def sync_ui(self):
        track = self.speaker.current_track
        if track != "None":
            idx = self.combo_tracks.findText(track)
            if idx != -1:
                self.combo_tracks.setCurrentIndex(idx)

        self.light_widget.update()
        self.socket_widget.update()
        self.vacuum_widget.update()
        self.speaker_widget.update()
        
        self.lbl_speaker_status.setText(
            f"Трек: {self.speaker.current_track} | "
            f"Vol: {self.speaker.volume}% | "
            f"[{'PLAY' if self.speaker.is_playing else 'PAUSE'}]"
        )
        self.speaker_controller.sync()