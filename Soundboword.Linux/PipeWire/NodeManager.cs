namespace Soundboword.Linux.PipeWire;

[RegisterSingleton(Registration = RegistrationStrategy.Self)]
public sealed partial class NodeManager : ObservableObject
{

    private readonly PipeWireCli? _cli;

    public NodeManager()
    {
    }

    public NodeManager(PipeWireCli cli)
    {
        _cli = cli;
        _ = Refresh();
    }

    public ObservableCollection<PipeWireNode> Nodes { get; } = [];

    [ObservableProperty]
    public partial PipeWireNode? SoundbowordNode { get; private set; }

    public async Task Refresh()
    {
        if (_cli == null)
            return;
        Nodes.Clear();
        SoundbowordNode = null;
        await _cli.IsAvailable;
        foreach (var node in await PipeWireCli.ListNodesAsync())
        {
            Nodes.Add(node);
            if (node is {Name: "Soundboword-Mic", Description: "Soundboword Microphone"})
                SoundbowordNode = node;
        }
    }

}
