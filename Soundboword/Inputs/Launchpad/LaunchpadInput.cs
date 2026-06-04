using SoundFlow.Midi.Routing;
using SoundFlow.Midi.Routing.Nodes;
using SoundFlow.Midi.Structs;

namespace Soundboword.Inputs.Launchpad;

public sealed class LaunchpadInput : IInputMethod
{

    private readonly MidiInputNode _node;

    public LaunchpadInput(MidiManager midi, MidiDeviceInfo input)
    {
        _node = midi.GetOrCreateInputNode(input);
        _node.OnMessageOutput += OnNodeOnOnMessageOutput;
    }

    private void OnNodeOnOnMessageOutput(MidiMessage message)
    {
        // TODO
    }

    public void Dispose() => _node.OnMessageOutput -= OnNodeOnOnMessageOutput;

}
