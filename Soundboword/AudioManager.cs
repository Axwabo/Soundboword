using System.Collections.Generic;
using System.IO;
using System.Linq;
using Soundboword.Models;
using Soundboword.ViewModels;
using SoundFlow.Abstracts.Devices;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Providers;
using SoundFlow.Structs;

namespace Soundboword;

public static class AudioManager
{

    private static MiniAudioEngine? _engine;
    private static AudioPlaybackDevice? _playback;

    private static readonly List<Sound> Sounds = [];

    private static readonly AudioFormat Format = new()
    {
        Format = SampleFormat.F32,
        SampleRate = 48000,
        Channels = 2
    };

    internal static void Init()
    {
        _engine = new MiniAudioEngine();
        var defaultDevice = _engine.PlaybackDevices.FirstOrDefault(e => e.IsDefault);
        _playback = _engine.InitializePlaybackDevice(defaultDevice, Format);
        _playback.Start();
    }

    internal static void Destroy()
    {
        _engine?.Dispose();
        _playback?.Dispose();
        foreach (var sound in Sounds)
        {
            sound.Player.Dispose();
            sound.Provider.Dispose();
        }

        Sounds.Clear();
    }

    public static void Play(SoundViewModel sound)
    {
        if (_engine == null || _playback == null)
            return;
        var provider = new StreamDataProvider(_engine, Format, File.OpenRead(sound.Path));
        var player = new SoundPlayer(_engine, Format, provider);
        _playback.MasterMixer.AddComponent(player);
        Sounds.Add(new Sound(provider, player));
        player.Play();
        player.PlaybackEnded += (sender, _) => Sounds.RemoveAll(e => e.Player == sender);
    }

}
