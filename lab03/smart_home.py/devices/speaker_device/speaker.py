from typing import Dict, Any, List
from core.device import Device

class Speaker(Device):
    def __init__(self, device_id: str, name: str):
        super().__init__(device_id, name)
        self.volume = 50
        self.is_playing = False
        self.current_track = "None"
        self.playlist: List[str] = []

    def get_state(self) -> Dict[str, Any]:
        """Сохраняем полный слепок состояния колонки."""
        return {
            "volume": self.volume,
            "is_playing": self.is_playing,
            "current_track": self.current_track
        }

    def set_state(self, state: Dict[str, Any]) -> None:
        """Восстанавливаем состояние из бэкапа."""
        self.volume = state.get("volume", self.volume)
        self.is_playing = state.get("is_playing", self.is_playing)
        self.current_track = state.get("current_track", self.current_track)
        
        status = "Играет" if self.is_playing else "Пауза"
        print(f"[Speaker:{self.name}] Состояние восстановлено: {self.current_track}, Громкость {self.volume}%, {status}")
    
    def set_volume(self, level: int):
        self.volume = level
        print(f"[Speaker:{self.name}] Громкость установлена на {level}%")

    def play(self):
        self.is_playing = True
        print(f"[Speaker:{self.name}] Воспроизведение начато")

    def pause(self):
        self.is_playing = False
        print(f"[Speaker:{self.name}] Воспроизведение приостановлено")

    def change_track(self, track_name: str):
        self.current_track = track_name
        print(f"[Speaker:{self.name}] Трек изменен на: {track_name}")

    def set_playlist(self, tracks: List[str]):
        """Загружает список доступных треков."""
        self.playlist = tracks

    def next_track(self):
        """Переключает на следующий трек по кругу."""
        if not self.playlist:
            return
        if self.current_track in self.playlist:
            idx = self.playlist.index(self.current_track)
            next_idx = (idx + 1) % len(self.playlist)
            self.current_track = self.playlist[next_idx]
        else:
            self.current_track = self.playlist[0]
        print(f"[Speaker:{self.name}] Следующий трек: {self.current_track}")

    def prev_track(self):
        """Переключает на предыдущий трек."""
        if not self.playlist:
            return
        if self.current_track in self.playlist:
            idx = self.playlist.index(self.current_track)
            prev_idx = (idx - 1) % len(self.playlist)
            self.current_track = self.playlist[prev_idx]
        else:
            self.current_track = self.playlist[-1]
        print(f"[Speaker:{self.name}] Предыдущий трек: {self.current_track}")