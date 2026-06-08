using System;
using System.Collections.Generic;
using Soundboword.Services;
using Soundboword.ViewModels;
using SoundFlow.Midi.Routing;
using SoundFlow.Midi.Routing.Nodes;
using SoundFlow.Midi.Structs;

namespace Soundboword.Inputs.Launchpad;

public sealed class LaunchpadInput : IInputMethod
{

    private readonly Dictionary<Guid, LaunchpadKey> _config;

    private readonly SoundList _list;

    private readonly MidiInputNode _node;

    private SoundViewModel? _listening;

    public LaunchpadInput(MidiManager midi, MidiDeviceInfo input, SoundList list)
    {
        _config = UserData.LoadLaunchpadConfig();
        _list = list;
        _node = midi.GetOrCreateInputNode(input);
        _node.OnMessageOutput += OnNodeOnOnMessageOutput;
    }

    public void ListenForShortcutAddition(SoundViewModel target) => _listening = target;

    public void ClearShortcut(SoundViewModel target) => _config.Remove(target.Id);

    public void CancelShortcutAddition() => _listening = null;

    public void Dispose()
    {
        _node.OnMessageOutput -= OnNodeOnOnMessageOutput;
        _listening = null;
        UserData.SaveLaunchpadConfig(_config);
    }

    private void OnNodeOnOnMessageOutput(MidiMessage message)
    {
        if (message.Velocity == 0)
            return;
        var key = message.LaunchpadKey;
        if (_listening != null)
        {
            _config[_listening.Id] = key;
            _list.Editor.CancelShortcutAddition();
            return;
        }

        foreach (var (guid, value) in _config)
        {
            if (value != key)
                continue;
            foreach (var sound in _list.Sounds)
                if (sound.Id == guid)
                    AudioManager.Trigger(sound);
        }
    }

}
