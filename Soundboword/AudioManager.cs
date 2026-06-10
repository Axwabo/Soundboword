using System;
using System.Collections.Generic;
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

public sealed class AudioManager
{

    private readonly MiniAudioEngine? _engine;
    private readonly AudioPlaybackDevice? _playback;

    private readonly Dictionary<SoundViewModel, List<SoundPlayback>> _sounds = [];

    private static readonly AudioFormat Format = new()
    {
        Format = SampleFormat.F32,
        SampleRate = 48000,
        Channels = 2
    };

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

    public MidiDeviceInfo[] RefreshMidiInputs()
    {
        if (_engine == null)
            return [];
        _engine.UpdateMidiDevicesInfo(); // TODO: portmidi does not support hotswap
        return _engine.MidiInputDevices;
    }

    public MidiManager? Midi => _engine?.MidiManager;

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
        foreach (var (provider, player) in list)
        {
            player.Stop();
            provider.Dispose();
            _playback!.MasterMixer.RemoveComponent(player);
        }
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
        list.Add(new SoundPlayback(provider, player));
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
        foreach (var component in _playback.MasterMixer.Components.ToList())
            _playback.MasterMixer.RemoveComponent(component);
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
            var removed = list.RemoveAll(e => e.Player == sender);
            if (removed == 0)
                continue;
            if (list.Count != 0)
                break;
            sound.PropertyChanged -= SoundOnPropertyChanged;
            sound.UpdatePlaybackState(SoundState.Stopped);
            _sounds.Remove(sound);
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
