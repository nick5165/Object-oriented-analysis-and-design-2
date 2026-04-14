from devices.speaker_device.speaker import Speaker
from core.command import Command

class ChangeVolumeCommand(Command):
    """Команда изменения громкости."""
    def __init__(self, app, device: Speaker, new_volume: int):
        super().__init__(app, device)
        self.new_volume = new_volume

    def execute(self) -> bool:
        if self.device.volume == self.new_volume:
            return False
            
        self.save_backup()
        self.device.set_volume(self.new_volume)
        return True

class PlayTrackCommand(Command):
    """Команда воспроизведения конкретного трека."""
    def __init__(self, app, device: Speaker, track_name: str):
        super().__init__(app, device)
        self.track_name = track_name

    def execute(self) -> bool:
        if self.device.current_track == self.track_name and self.device.is_playing:
            return False
            
        self.save_backup()
        self.device.change_track(self.track_name)
        self.device.play()
        return True

class MediaPauseCommand(Command):
    """Команда паузы."""
    def execute(self) -> bool:
        if not self.device.is_playing:
            return False
            
        self.save_backup()
        self.device.pause()
        return True

class SpeakerNextCommand(Command):
    """Команда переключения на следующий трек."""
    def execute(self) -> bool:
        self.save_backup()
        self.device.next_track()
        self.device.play()
        return True