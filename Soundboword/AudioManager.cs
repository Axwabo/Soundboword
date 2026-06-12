using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using Soundboword.Models;
using Soundboword.Services;
using Soundboword.ViewModels;
using SoundFlow.Enums;
using SoundFlow.Structs;

namespace Soundboword;

// TODO: thread safety
public sealed class AudioManager
{

    private readonly SoundFlowDeviceManager _deviceManager;

    private static readonly AudioFormat Format = new()
    {
        Format = SampleFormat.F32,
        SampleRate = 48000,
        Channels = 2
    };

    private readonly Dictionary<SoundViewModel, List<SoundPlayback>> _sounds = [];

    public AudioManager(SoundFlowDeviceManager deviceManager)
    {
        _deviceManager = deviceManager;
    }

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
                StopAll(list);
                sound.PropertyChanged -= SoundOnPropertyChanged;
                sound.UpdatePlaybackState(SoundState.Stopped);
                _sounds.Remove(sound);
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

    public void TogglePause(SoundViewModel sound)
    {
        if (!_sounds.TryGetValue(sound, out var list))
            return;
        var pause = sound.PlaybackState == SoundState.Playing;
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
        if (sound.PlaybackState == SoundState.Paused)
            return;
        soundPlayback.Player.Play();
        sound.UpdatePlaybackState(SoundState.Playing);
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
    }

}

file static class SoundExtensions
{

    extension(SoundViewModel sound)
    {

        public bool IsActive(Dictionary<SoundViewModel, List<SoundPlayback>> sounds) => sounds.TryGetValue(sound, out var list) && list.Count != 0;

    }

}
