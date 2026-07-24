namespace Soundboword.Linux.PipeWire;

[RegisterSingleton(Registration = RegistrationStrategy.Self)]
public sealed partial class NodeManager : ObservableObject
{

    private const string Stream = "Stream/Output/Audio";

    private static readonly Comparison<PipeWirePort> PortComparison = (a, b) => a.PortId.CompareTo(b.PortId);

    private readonly PipeWireCli _cli;
    private readonly SoundFlowDeviceManager _outputManager;

    private readonly List<PipeWireNode> _sinks = [];

    private Task? _relinkTask;

    public NodeManager(PipeWireCli cli, SoundFlowDeviceManager outputManager)
    {
        _cli = cli;
        _outputManager = outputManager;
        _ = Refresh();
        outputManager.DeviceSwitched += () =>
        {
            if (_relinkTask is not {IsCompleted: false})
                _relinkTask = RelinkAfterDeviceSwitch();
        };
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
                case PipeWireNode {Class: Stream} playback when playback.Name.StartsWith("Soundboword"):
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

    private async Task RelinkAfterDeviceSwitch()
    {
        await Task.Delay(100);
        var objects = await PipeWireCli.ListObjectsAsync();
        var links = new List<PipeWireLink>();
        var mic = Task.CompletedTask;
        var hearSounds = Task.CompletedTask;
        var micSoundsOn = MicSounds?.IsLinked ?? true;
        RefreshPlaybackAndPortsAndLinks(objects, links);
        if (PlaybackNode is not null && MicNode is not null)
        {
            MicSounds = NodeLinkManager.Create(PlaybackNode, MicNode, Ports, links);
            mic = MicSounds?.ToggleLink(micSoundsOn, links) ?? Task.CompletedTask;
        }
        else
            MicSounds = null;

        var output = _outputManager.SelectedDevice.Name;
        foreach (var node in _sinks)
        {
            if (node.Description != output)
                continue;
            hearSounds = node == OutputNode
                ? HearSounds?.EnsureState(links) ?? Task.CompletedTask
                : Relink(node, links);
            break;
        }

        await Task.WhenAll(mic, hearSounds);
    }

    private void RefreshPlaybackAndPortsAndLinks(List<PipeWireObject> objects, List<PipeWireLink> links)
    {
        PlaybackNode = null;
        Ports.Clear();
        foreach (var pwObj in objects)
            switch (pwObj)
            {
                case PipeWireNode {Class: Stream} node:
                    PlaybackNode = node;
                    break;
                case PipeWirePort port:
                    Ports.Add(port);
                    break;
                case PipeWireLink link:
                    links.Add(link);
                    break;
            }

        Ports.Sort(PortComparison);
    }

    private async Task Relink(PipeWireNode node, List<PipeWireLink> links)
    {
        var linked = HearSounds?.IsLinked ?? true;
        OutputNode = node;
        if (PlaybackNode is not null)
            HearSounds = NodeLinkManager.Create(PlaybackNode, node, Ports, links);
        if (HearSounds != null)
            await HearSounds.ToggleLink(linked, links);
    }

}
