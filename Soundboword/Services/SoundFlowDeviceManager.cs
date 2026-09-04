using System.Diagnostics.CodeAnalysis;
using SoundFlow.Abstracts.Devices;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Midi.PortMidi;
using SoundFlow.Midi.Routing;
using SoundFlow.Midi.Structs;
using SoundFlow.Providers;
using SoundFlow.Structs;

namespace Soundboword.Services;

[RegisterSingleton]
public sealed class SoundFlowDeviceManager : IDisposable
{

    private const string FileName = "output";

    private static readonly AudioFormat Format = new()
    {
        Format = SampleFormat.F32,
        SampleRate = 48000,
        Channels = 2
    };

    private MiniAudioEngine? _engine;
    private AudioPlaybackDevice? _playback;

    public SoundFlowDeviceManager() : this(new UserData(), new Lifetime())
    {
    }

    public SoundFlowDeviceManager(UserData data, Lifetime lifetime)
    {
        if (!lifetime.IsActive)
        {
            _engine = null;
            return;
        }

        InitializeEngine();
        var preferredDeviceName = data.Load(FileName);
        var preferredDevice = _engine.PlaybackDevices.FirstOrDefault(e => e.Name.AsSpan().Trim().Equals(preferredDeviceName.AsSpan().Trim(), StringComparison.OrdinalIgnoreCase));
        SwitchDevice(preferredDevice != default ? preferredDevice : _engine.PlaybackDevices.First(e => e.IsDefault));
        lifetime.Exit += () =>
        {
            data.Save(FileName, SelectedDevice.Name);
            Dispose();
        };
    }

    [MemberNotNullWhen(true, nameof(_engine))]
    public bool IsInitialized => _engine != null;

    public DeviceInfo SelectedDevice { get; private set; }

    public ObservableCollection<DeviceInfo> Devices { get; } = [];

    public MidiManager? Midi => _engine?.MidiManager;

    public void Dispose()
    {
        StopAll();
        _playback?.Dispose();
        _engine?.Dispose();
        _playback = null;
        _engine = null;
    }

    public event Action? OutputDeviceSwitched;

    public event Action? MicrophoneSwitched;

    [MemberNotNull(nameof(_engine))]
    public void InitializeEngine()
    {
        if (IsInitialized)
            return;
        _engine = new MiniAudioEngine();
        _engine.UsePortMidi();
        _engine.UpdateAudioDevicesInfo();
        foreach (var device in _engine.PlaybackDevices)
            Devices.Add(device);
    }

    public void SwitchDevice(DeviceInfo info)
    {
        if (!IsInitialized)
            return;
        if (_playback != null)
            _playback = _engine.SwitchDevice(_playback, info);
        else
        {
            _playback = _engine.InitializePlaybackDevice(info, Format);
            _playback.Start();
        }

        SelectedDevice = info;
        OutputDeviceSwitched?.Invoke();
    }

    public void SwitchToDefaultDevice()
    {
        if (IsInitialized && _playback == null)
            SwitchDevice(_engine.PlaybackDevices.First(e => e.IsDefault));
    }

    public void InvokeMicrophoneSwitched() => MicrophoneSwitched?.Invoke();

    public void RefreshAudioDevices()
    {
        if (!IsInitialized)
            return;
        _engine.UpdateAudioDevicesInfo();
        Devices.Clear();
        foreach (var device in _engine.PlaybackDevices)
            Devices.Add(device);
    }

    public MidiDeviceInfo[] RefreshMidiInputs()
    {
        if (!IsInitialized)
            return [];
        _engine.UpdateMidiDevicesInfo(); // TODO: portmidi does not support hotswap
        return _engine.MidiInputDevices;
    }

    public SoundPlayback InitializePlayback(SoundViewModel sound)
    {
        ObjectDisposedException.ThrowIf(!IsInitialized, this);
        var provider = new StreamDataProvider(_engine, Format, File.OpenRead(sound.Path));
        var player = new SoundPlayer(_engine, Format, provider);
        _playback!.MasterMixer.AddComponent(player);
        player.Volume = sound.Volume;
        player.IsLooping = sound.Loop;
        if (sound.Edits is {HasStart: true, Start: var start})
            player.Seek(start);
        if (sound.Edits is {HasLoopStart: true, LoopStart: var loopStart, HasLoopEnd: var hasLoopEnd, LoopEnd: var loopEnd})
            player.SetLoopPoints(loopStart, hasLoopEnd ? loopEnd : null);
        return new SoundPlayback(provider, player, sound.Name);
    }

    public void Stop(SoundPlayback playback)
    {
        playback.Player.Dispose();
        _playback?.MasterMixer.RemoveComponent(playback.Player);
    }

    public void StopAll()
    {
        if (_playback == null)
            return;
        foreach (var component in _playback.MasterMixer.Components.ToList())
        {
            component.Dispose();
            _playback.MasterMixer.RemoveComponent(component);
        }
    }

}
