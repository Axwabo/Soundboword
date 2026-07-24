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

    private PipeWireNode? OutputNode { get; set; }

    private PipeWireNode? PlaybackNode { get; set; }

    [ObservableProperty]
    public partial PipeWireNode? PhysicalMicrophone { get; set; }

    [ObservableProperty]
    public partial NodeLinkManager? HearSounds { get; private set; }

    [ObservableProperty]
    public partial NodeLinkManager? MicSounds { get; private set; }

    [ObservableProperty]
    public partial NodeLinkManager? MicPassthrough { get; private set; }

    [ObservableProperty]
    public partial NodeLinkManager? HearMyself { get; private set; }

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
        if (PlaybackNode is not null && OutputNode is not null)
            HearSounds = NodeLinkManager.Create(PlaybackNode, OutputNode, Ports, links);
        if (PlaybackNode is not null && MicNode is not null)
            MicSounds = NodeLinkManager.Create(PlaybackNode, MicNode, Ports, links);
        if (PhysicalMicrophone is not null && MicNode is not null)
            MicPassthrough = NodeLinkManager.Create(PhysicalMicrophone, MicNode, Ports, links);
        if (PhysicalMicrophone is not null && OutputNode is not null)
            HearMyself = NodeLinkManager.Create(PhysicalMicrophone, OutputNode, Ports, links);
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
        MicPassthrough?.EnsureState();
        MicSounds?.EnsureState();
        var output = _outputManager.SelectedDevice.Name;
        foreach (var node in _sinks)
        {
            if (node.Description != output)
                continue;
            if (node == OutputNode)
                break;
            var linked = HearSounds?.IsLinked ?? true;
            HearSounds?.ToggleLink(false);
            if (PlaybackNode is not null)
                HearSounds = NodeLinkManager.Create(PlaybackNode, node, Ports, []);
            HearSounds?.ToggleLink(linked);
            OutputNode = node;
            return;
        }

        HearSounds?.EnsureState();
    }

}
