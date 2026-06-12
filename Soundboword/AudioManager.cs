using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Avalonia.Controls.ApplicationLifetimes;
using Soundboword.Models;
using Soundboword.ViewModels;
using SoundFlow.Abstracts.Devices;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Midi.PortMidi;
using SoundFlow.Midi.Routing;
using SoundFlow.Midi.Structs;
using SoundFlow.Providers;
using SoundFlow.Structs;

namespace Soundboword;

// TODO: thread safety
public sealed class AudioManager
{

    private static readonly AudioFormat Format = new()
    {
        Format = SampleFormat.F32,
        SampleRate = 48000,
        Channels = 2
    };

    private readonly MiniAudioEngine? _engine;
    private readonly AudioPlaybackDevice? _playback;

    private readonly Dictionary<SoundViewModel, List<SoundPlayback>> _sounds = [];

    public AudioManager(IClassicDesktopStyleApplicationLifetime? lifetime = null)
    {
        if (lifetime == null)
            return;
        _engine = new MiniAudioEngine();
        _engine.UsePortMidi();
        var defaultDevice = _engine.PlaybackDevices.FirstOrDefault(e => e.IsDefault);
        _playback = _engine.InitializePlaybackDevice(defaultDevice, Format);
        _playback.Start();
        lifetime.Exit += (_, _) => Destroy();
    }

    public ObservableCollection<SoundPlayback> AllSounds { get; } = [];

    public MidiManager? Midi => _engine?.MidiManager;

    public MidiDeviceInfo[] RefreshMidiInputs()
    {
        if (_engine == null)
            return [];
        _engine.UpdateMidiDevicesInfo(); // TODO: portmidi does not support hotswap
        return _engine.MidiInputDevices;
    }

    private void Destroy()
    {
        _engine?.Dispose();
        _playback?.Dispose();
        foreach (var list in _sounds.Values)
        foreach (var sound in list)
        {
            sound.Player.Dispose();
            sound.Provider.Dispose();
        }

        _sounds.Clear();
    }

    public void Trigger(SoundViewModel sound)
    {
        if (_engine == null || _playback == null)
            return;
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
        played.Player.Stop();
        played.Provider.Dispose();
        _playback!.MasterMixer.RemoveComponent(played.Player);
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

        var provider = new StreamDataProvider(_engine!, Format, File.OpenRead(sound.Path));
        var player = new SoundPlayer(_engine!, Format, provider);
        _playback!.MasterMixer.AddComponent(player);
        var soundPlayback = new SoundPlayback(provider, player, sound.Name);
        list.Add(soundPlayback);
        AllSounds.Add(soundPlayback);
        player.Volume = sound.Volume;
        player.IsLooping = sound.Loop;
        player.PlaybackEnded += RemoveSoundOnEnd;
        if (sound.PlaybackState == SoundState.Paused)
            return;
        player.Play();
        sound.UpdatePlaybackState(SoundState.Playing);
    }

    public void StopAll()
    {
        if (_playback == null)
            return;
        AllSounds.Clear();
        foreach (var component in _playback.MasterMixer.Components.ToList())
        {
            component.Dispose();
            _playback.MasterMixer.RemoveComponent(component);
        }

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
