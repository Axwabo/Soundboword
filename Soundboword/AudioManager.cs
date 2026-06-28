namespace Soundboword;

// TODO: thread safety
[RegisterSingleton]
public sealed class AudioManager
{

    private readonly SoundFlowDeviceManager _deviceManager;

    private readonly Dictionary<SoundViewModel, List<SoundPlayback>> _sounds = [];

    public AudioManager(SoundFlowDeviceManager deviceManager) => _deviceManager = deviceManager;

    public ObservableCollection<SoundPlayback> AllSounds { get; } = [];

    public void Trigger(SoundViewModel sound)
    {
        switch (sound.Mode)
        {
            case TriggerMode.Duplicate:
            case TriggerMode.StartRestart or TriggerMode.StartStop or TriggerMode.PlayPause when !sound.IsActive(_sounds):
            {
                PlayNew(sound);
                break;
            }
            case TriggerMode.PlayPause:
                TogglePause(sound);
                break;
            case TriggerMode.StartStop when _sounds.TryGetValue(sound, out var list):
                StopAll(sound, list);
                break;
            case TriggerMode.StartRestart when _sounds.TryGetValue(sound, out var list):
                foreach (var played in list)
                {
                    played.Player.Play();
                    played.Provider.Seek(0);
                }

                break;
        }
    }

    private void StopAll(SoundViewModel sound, List<SoundPlayback> list)
    {
        StopAll(list);
        sound.PropertyChanged -= SoundOnPropertyChanged;
        sound.UpdatePlaybackState(SoundState.Stopped);
        _sounds.Remove(sound);
        ResumeOthers(sound);
    }

    public void TogglePause(SoundViewModel sound)
    {
        if (!_sounds.TryGetValue(sound, out var list))
            return;
        if (sound.PlaybackState == SoundState.Playing)
            sound.Pause(IPlaybackSuspender.User);
        else
            sound.Resume(IPlaybackSuspender.User);
        UpdatePausedState(sound, list);
    }

    private static void UpdatePausedState(SoundViewModel sound, List<SoundPlayback> list)
    {
        var pause = sound.IsPaused;
        foreach (var playback in list)
            if (pause)
                playback.Player.Pause();
            else
                playback.Player.Play();
        sound.UpdatePlaybackState(pause ? SoundState.Paused : SoundState.Playing);
    }

    private void StopAll(List<SoundPlayback> list)
    {
        foreach (var played in list)
            StopInternal(played);
    }

    private void StopInternal(SoundPlayback played)
    {
        AllSounds.Remove(played);
        _deviceManager.Stop(played);
    }

    private void PlayNew(SoundViewModel sound)
    {
        if (!File.Exists(sound.Path))
        {
            sound.UpdatePlaybackState(SoundState.Error);
            return;
        }

        if (!_sounds.TryGetValue(sound, out var list))
        {
            list = _sounds[sound] = [];
            sound.PropertyChanged += SoundOnPropertyChanged;
        }

        var soundPlayback = _deviceManager.InitializePlayback(sound);
        list.Add(soundPlayback);
        AllSounds.Add(soundPlayback);
        soundPlayback.Player.PlaybackEnded += RemoveSoundOnEnd;
        ApplyInteraction(sound);
        if (sound.PlaybackState == SoundState.Paused)
            return;
        soundPlayback.Player.Play();
        sound.UpdatePlaybackState(SoundState.Playing);
    }

    private void ApplyInteraction(SoundViewModel sound)
    {
        switch (sound.Interaction)
        {
            case OtherSoundInteraction.Nothing:
                break;
            case OtherSoundInteraction.Stop:
                foreach (var (other, list) in _sounds.ToDictionary())
                    if (other != sound)
                        StopAll(other, list);
                break;
            case OtherSoundInteraction.Pause:
                foreach (var (other, list) in _sounds)
                {
                    if (other == sound)
                        continue;
                    other.Pause(sound);
                    UpdatePausedState(other, list);
                }

                break;
            case OtherSoundInteraction.Mute:
                // TODO
                break;
        }
    }

    public void StopAll()
    {
        AllSounds.Clear();
        _deviceManager.StopAll();
        foreach (var sound in _sounds.Keys)
        {
            sound.PropertyChanged -= SoundOnPropertyChanged;
            sound.UpdatePlaybackState(SoundState.Stopped);
        }

        _sounds.Clear();
    }

    private void RemoveSoundOnEnd(object? sender, EventArgs _)
    {
        // TODO: optimize
        foreach (var (sound, list) in _sounds)
        {
            var index = list.FindIndex(e => e.Player == sender);
            if (index == -1)
                continue;
            var playback = list[index];
            _deviceManager.Stop(playback);
            list.RemoveAt(index);
            MarkRemoved(playback, list, sound);
            break;
        }
    }

    private void MarkRemoved(SoundPlayback playback, List<SoundPlayback> list, SoundViewModel sound)
    {
        AllSounds.Remove(playback);
        if (list.Count != 0)
            return;
        sound.PropertyChanged -= SoundOnPropertyChanged;
        sound.UpdatePlaybackState(SoundState.Stopped);
        _sounds.Remove(sound);
        ResumeOthers(sound);
    }

    public void Stop(SoundPlayback playback)
    {
        StopInternal(playback);
        // TODO: optimize
        foreach (var (sound, list) in _sounds)
        {
            if (!list.Remove(playback))
                continue;
            AllSounds.Remove(playback);
            MarkRemoved(playback, list, sound);
            break;
        }
    }

    private void SoundOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not SoundViewModel sound || !_sounds.TryGetValue(sound, out var list))
            return;
        switch (e.PropertyName)
        {
            case nameof(SoundViewModel.Volume):
                foreach (var played in list)
                    played.Player.Volume = sound.Volume;
                break;
            case nameof(SoundViewModel.Loop):
            {
                foreach (var played in list)
                    played.Player.IsLooping = sound.Loop;
                break;
            }
        }
    }

    public void StopAll(SoundViewModel sound)
    {
        if (!_sounds.Remove(sound, out var list))
            return;
        StopAll(list);
        sound.PropertyChanged -= SoundOnPropertyChanged;
        sound.UpdatePlaybackState(SoundState.Stopped);
        ResumeOthers(sound);
    }

    private void ResumeOthers(SoundViewModel sound)
    {
        foreach (var (other, playbacks) in _sounds)
        {
            other.Resume(sound);
            UpdatePausedState(other, playbacks);
        }
    }

}

file static class SoundExtensions
{

    extension(SoundViewModel sound)
    {

        public bool IsActive(Dictionary<SoundViewModel, List<SoundPlayback>> sounds) => sounds.TryGetValue(sound, out var list) && list.Count != 0;

    }

}
