using Avalonia.Threading;
using SoundFlow.Midi.Routing;
using SoundFlow.Midi.Routing.Nodes;
using SoundFlow.Midi.Structs;

namespace Soundboword.Inputs.Launchpad;

public sealed class LaunchpadInput : IInputMethod
{

    public const string Name = "Launchpad Mini";

    private readonly MidiInputNode _node;

    private readonly ShortcutList _shortcuts;

    public LaunchpadInput(MidiManager midi, MidiDeviceInfo input, ShortcutList shortcuts)
    {
        _shortcuts = shortcuts;
        _node = midi.GetOrCreateInputNode(input);
        _node.OnMessageOutput += OnNodeOnOnMessageOutput;
    }

    public void Dispose() => _node.OnMessageOutput -= OnNodeOnOnMessageOutput;

    private void OnNodeOnOnMessageOutput(MidiMessage message)
    {
        if (message.Velocity == 0)
            return;
        var key = message.LaunchpadKey;
        Dispatcher.UIThread.Post(() => _shortcuts.Trigger(key, Name));
    }

}
