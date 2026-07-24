namespace Soundboword.Linux.PipeWire;

[RegisterSingleton(Registration = RegistrationStrategy.Self)]
public sealed partial class NodeManager : ObservableObject
{

    private static readonly Comparison<PipeWirePort> PortComparison = (a, b) => a.PortId.CompareTo(b.PortId);

    private readonly PipeWireCli _cli;
    private readonly SoundFlowDeviceManager _outputManager;

    private readonly List<PipeWireNode> _sinks = [];

    public NodeManager(PipeWireCli cli, SoundFlowDeviceManager outputManager)
    {
        _cli = cli;
        _outputManager = outputManager;
        _ = Refresh();
        outputManager.DeviceSwitched += OutputManagerOnDeviceSwitched;
    }

    public ObservableCollection<PipeWireNode> Sources { get; } = [];

    public List<PipeWirePort> Ports { get; } = [];

    public PipeWireNode? MicNode { get; private set; }

    public PipeWireNode? OutputNode { get; private set; }

    [ObservableProperty]
    public partial PipeWireNode? PhysicalMicrophone { get; set; }

    public PipeWireNode? PlaybackNode { get; private set; }

    [ObservableProperty]
    public partial NodeLinkManager? PhysicalToVirtual { get; private set; }

    [ObservableProperty]
    public partial NodeLinkManager? PlaybackToVirtual { get; private set; }

    public async Task Refresh()
    {
        _sinks.Clear();
        Sources.Clear();
        Ports.Clear();
        MicNode = null;
        PlaybackNode = null;
        OutputNode = null;
        await _cli.IsAvailable;
        var objects = await PipeWireCli.ListObjectsAsync();
        var links = new List<PipeWireLink>();
        RefreshObjects(objects, links);
        Ports.Sort(PortComparison);
        PhysicalMicrophone ??= Sources.Count == 0 ? null : Sources[0];
        if (MicNode is null)
            return;
        if (PhysicalMicrophone is not null)
            PhysicalToVirtual = NodeLinkManager.Create(PhysicalMicrophone, MicNode, Ports, links);
        if (PlaybackNode is not null)
            PlaybackToVirtual = NodeLinkManager.Create(PlaybackNode, MicNode, Ports, links);
    }

    private void RefreshObjects(List<PipeWireObject> objects, List<PipeWireLink> links)
    {
        var selectedOutput = _outputManager.SelectedDevice.Name;
        foreach (var pwObj in objects)
            switch (pwObj)
            {
                case PipeWireNode {Class: "Stream/Output/Audio"} playback when playback.Name.StartsWith("Soundboword"):
                    PlaybackNode = playback;
                    break;
                case PipeWireNode {Class: "Audio/Source/Virtual", Name: "Soundboword-Mic", Description: "Soundboword Microphone"} mic:
                    MicNode = mic;
                    break;
                case PipeWireNode {Class: "Audio/Duplex"} duplex:
                    Sources.Add(duplex);
                    if (duplex.Description == selectedOutput)
                        OutputNode = duplex;
                    break;
                case PipeWireNode {Class: "Audio/Sink"} sink:
                    _sinks.Add(sink);
                    if (sink.Description == selectedOutput)
                        OutputNode = sink;
                    break;
                case PipeWireNode {Class: "Audio/Source"} source:
                    Sources.Add(source);
                    break;
                case PipeWirePort port:
                    Ports.Add(port);
                    break;
                case PipeWireLink link:
                    links.Add(link);
                    break;
            }
    }

    private void OutputManagerOnDeviceSwitched()
    {
        PhysicalToVirtual?.EnsureState();
        PlaybackToVirtual?.EnsureState();
    }

}
