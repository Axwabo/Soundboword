namespace Soundboword.Linux.PipeWire;

[RegisterSingleton(Registration = RegistrationStrategy.Self)]
public sealed partial class NodeManager : ObservableObject
{

    private readonly PipeWireCli _cli;

    public NodeManager(PipeWireCli cli)
    {
        _cli = cli;
        _ = Refresh();
    }

    public ObservableCollection<PipeWireNode> Nodes { get; } = [];

    public ObservableCollection<PipeWireNode> Microphones { get; } = [];

    [ObservableProperty]
    public partial PipeWireNode? SoundbowordNode { get; private set; }

    [ObservableProperty]
    public partial PipeWireNode? PhysicalMicrophone { get; set; }

    public async Task Refresh()
    {
        Nodes.Clear();
        Microphones.Clear();
        SoundbowordNode = null;
        await _cli.IsAvailable;
        foreach (var node in await PipeWireCli.ListNodesAsync())
        {
            Nodes.Add(node);
            if (node is {Name: "Soundboword-Mic", Description: "Soundboword Microphone"})
                SoundbowordNode = node;
            else if (node.Class is "Audio/Source" or "Audio/Duplex") // TODO: should we allow duplex devices?
                Microphones.Add(node);
        }
    }

}
