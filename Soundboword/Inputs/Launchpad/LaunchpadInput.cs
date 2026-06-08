using System;
using Soundboword.Services;
using Soundboword.ViewModels;
using SoundFlow.Midi.Routing;
using SoundFlow.Midi.Routing.Nodes;
using SoundFlow.Midi.Structs;

namespace Soundboword.Inputs.Launchpad;

public sealed class LaunchpadInput : IInputMethod
{

    private readonly SoundList _list;

    private readonly MidiInputNode _node;

    private SoundViewModel? _listening;

    public LaunchpadInput(MidiManager midi, MidiDeviceInfo input, SoundList list)
    {
        _list = list;
        _node = midi.GetOrCreateInputNode(input);
        _node.OnMessageOutput += OnNodeOnOnMessageOutput;
    }

    public void ListenForShortcutAddition(SoundViewModel target) => _listening = target;

    public void CancelShortcutAddition() => _listening = null;

    public void Dispose() => _node.OnMessageOutput -= OnNodeOnOnMessageOutput;

    private void OnNodeOnOnMessageOutput(MidiMessage message)
    {
        Console.WriteLine(message.LaunchpadKey);
    }

}
