using Avalonia.Threading;

namespace Soundboword.Linux.PipeWire;

[RegisterSingleton(Registration = RegistrationStrategy.Self)]
public sealed partial class NodeManager : ObservableObject
{

    private static readonly Comparison<PipeWirePort> PortComparison = (a, b) => a.PortId.CompareTo(b.PortId);

    private readonly PipeWireCli _cli;

    public NodeManager(PipeWireCli cli)
    {
        _cli = cli;
        _ = Refresh();
    }

    public List<PipeWirePort> Ports { get; } = [];

    public ObservableCollection<PipeWireNode> Microphones { get; } = [];

    [ObservableProperty]
    public partial PipeWireNode? MicNode { get; private set; }

    [ObservableProperty]
    public partial PipeWireNode? PhysicalMicrophone { get; set; }

    public PipeWireNode? PlaybackNode { get; private set; }

    [ObservableProperty]
    public partial NodeLinkManager? PhysicalToVirtual { get; private set; }

    public event Action? ObjectsRefreshed;

    public async Task Refresh()
    {
        Ports.Clear();
        Microphones.Clear();
        MicNode = null;
        PlaybackNode = null;
        await _cli.IsAvailable;
        var objects = await PipeWireCli.ListObjectsAsync();
        foreach (var pwObj in objects)
            switch (pwObj)
            {
                case PipeWireNode {Class: "Stream/Output/Audio", Name: var name} node when name.StartsWith("Soundboword"):
                    PlaybackNode = node;
                    break;
                case PipeWireNode {Class: "Audio/Source/Virtual", Name: "Soundboword-Mic", Description: "Soundboword Microphone"} node:
                    MicNode = node;
                    break;
                // TODO: should we allow duplex devices?
                case PipeWireNode {Class: "Audio/Source" or "Audio/Duplex"} node:
                    Microphones.Add(node);
                    break;
                case PipeWirePort port:
                    Ports.Add(port);
                    break;
            }

        Ports.Sort(PortComparison);
        if (ObjectsRefreshed != null)
            Dispatcher.UIThread.InvokeOrPost(ObjectsRefreshed);
        if (PhysicalMicrophone is not null && MicNode is not null)
            PhysicalToVirtual = NodeLinkManager.Create(PhysicalMicrophone, MicNode, Ports);
    }

}
