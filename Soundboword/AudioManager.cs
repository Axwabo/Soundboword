using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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

public static class AudioManager
{

    private static MiniAudioEngine? _engine;
    private static AudioPlaybackDevice? _playback;

    private static readonly Dictionary<SoundViewModel, List<SoundPlayback>> Sounds = [];

    private static readonly AudioFormat Format = new()
    {
        Format = SampleFormat.F32,
        SampleRate = 48000,
        Channels = 2
    };

    internal static void Init()
    {
        _engine = new MiniAudioEngine();
        _engine.UsePortMidi();
        var defaultDevice = _engine.PlaybackDevices.FirstOrDefault(e => e.IsDefault);
        _playback = _engine.InitializePlaybackDevice(defaultDevice, Format);
        _playback.Start();
    }

    public static MidiDeviceInfo[] RefreshMidiInputs()
    {
        if (_engine == null)
            return [];
        _engine.UpdateMidiDevicesInfo(); // TODO: portmidi does not support hotswap
        return _engine.MidiInputDevices;
    }

    public static MidiManager? Midi => _engine?.MidiManager;

    internal static void Destroy()
    {
        _engine?.Dispose();
        _playback?.Dispose();
        foreach (var list in Sounds.Values)
        foreach (var sound in list)
        {
            sound.Player.Dispose();
            sound.Provider.Dispose();
        }

        Sounds.Clear();
    }

    public static void Trigger(SoundViewModel sound)
    {
        if (_engine == null || _playback == null)
            return;
        switch (sound.Mode)
        {
            case TriggerMode.Duplicate:
            case TriggerMode.StartRestart or TriggerMode.StartStop or TriggerMode.PlayPause when !sound.Active:
            {
                PlayNew(sound);
                break;
            }
            case TriggerMode.PlayPause:
                TogglePause(sound);
                break;
            case TriggerMode.StartStop when Sounds.TryGetValue(sound, out var list):
                StopAll(list);
                sound.PropertyChanged -= SoundOnPropertyChanged;
                sound.UpdatePlaybackState(SoundState.Stopped);
                Sounds.Remove(sound);
                break;
            case TriggerMode.StartRestart when Sounds.TryGetValue(sound, out var list):
                foreach (var played in list)
                {
                    played.Player.Play();
                    played.Provider.Seek(0);
                }

                break;
        }
    }

    public static void TogglePause(SoundViewModel sound)
    {
        if (!Sounds.TryGetValue(sound, out var list))
            return;
        foreach (var playback in list)
            if (playback.Player.State == PlaybackState.Playing)
                playback.Player.Pause();
            else
                playback.Player.Play();
    }

    private static void StopAll(List<SoundPlayback> list)
    {
        foreach (var (provider, player) in list)
        {
            player.Stop();
            provider.Dispose();
            _playback!.MasterMixer.RemoveComponent(player);
        }
    }

    private static void PlayNew(SoundViewModel sound)
    {
        if (!Sounds.TryGetValue(sound, out var list))
        {
            list = Sounds[sound] = [];
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

    internal static void StopAll()
    {
        if (_playback == null)
            return;
        foreach (var component in _playback.MasterMixer.Components.ToList())
            _playback.MasterMixer.RemoveComponent(component);
        foreach (var sound in Sounds.Keys)
        {
            sound.PropertyChanged -= SoundOnPropertyChanged;
            sound.UpdatePlaybackState(SoundState.Stopped);
        }

        Sounds.Clear();
    }

    private static void RemoveSoundOnEnd(object? sender, EventArgs _)
    {
        foreach (var (sound, list) in Sounds)
        {
            var removed = list.RemoveAll(e => e.Player == sender);
            if (removed == 0)
                continue;
            sound.PropertyChanged -= SoundOnPropertyChanged;
            sound.UpdatePlaybackState(SoundState.Stopped);
            Sounds.Remove(sound);
            break;
        }
    }

    private static void SoundOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not SoundViewModel sound || !Sounds.TryGetValue(sound, out var list))
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

    public static void StopAll(SoundViewModel sound)
    {
        if (!Sounds.Remove(sound, out var list))
            return;
        StopAll(list);
        sound.PropertyChanged -= SoundOnPropertyChanged;
        sound.UpdatePlaybackState(SoundState.Stopped);
    }

    extension(SoundViewModel sound)
    {

        private bool Active => Sounds.TryGetValue(sound, out var list) && list.Count != 0;

    }

}
