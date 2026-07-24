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

    public ObservableCollection<PipeWireNode> Microphones { get; } = [];

    [ObservableProperty]
    public partial PipeWireNode? MicNode { get; private set; }

    [ObservableProperty]
    public partial PipeWireNode? PlaybackNode { get; private set; }

    [ObservableProperty]
    public partial PipeWireNode? PhysicalMicrophone { get; set; }

    public async Task Refresh()
    {
        Microphones.Clear();
        MicNode = null;
        PlaybackNode = null;
        await _cli.IsAvailable;
        foreach (var node in await PipeWireCli.ListNodesAsync())
            switch (node.Class)
            {
                case "Stream/Output/Audio" when node.Name.StartsWith("Soundboword"):
                    PlaybackNode = node;
                    break;
                case "Audio/Source/Virtual" when node is {Name: "Soundboword-Mic", Description: "Soundboword Microphone"}:
                    MicNode = node;
                    break;
                // TODO: should we allow duplex devices?
                case "Audio/Source" or "Audio/Duplex":
                    Microphones.Add(node);
                    break;
            }
    }

}
