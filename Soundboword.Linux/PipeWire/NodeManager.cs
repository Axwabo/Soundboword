namespace Soundboword.Linux.PipeWire;

[RegisterSingleton(Registration = RegistrationStrategy.Self)]
public sealed partial class NodeManager : ObservableObject
{

    private const string Stream = "Stream/Output/Audio";

    private static readonly Comparison<PipeWirePort> PortComparison = (a, b) => a.PortId.CompareTo(b.PortId);

    private readonly PipeWireCli _cli;
    private readonly SoundFlowDeviceManager _outputManager;

    private Task? _relinkTask;

    public NodeManager(PipeWireCli cli, DevicesViewModel devicesViewModel)
    {
        _cli = cli;
        _outputManager = devicesViewModel.DeviceManager;
        _ = Refresh();
        devicesViewModel.DeviceSwitched = () =>
        {
            if (_relinkTask is not {IsCompleted: false})
                _relinkTask = RelinkAfterDeviceSwitch();
            return _relinkTask;
        };
    }

    public ObservableCollection<PipeWireNode> Sources { get; } = [];

    private List<PipeWirePort> Ports { get; } = [];

    private List<PipeWireLink> Links { get; } = [];

    private PipeWireNode? OutputNode { get; set; }

    private PipeWireNode? PlaybackNode { get; set; }

    [ObservableProperty]
    public partial PipeWireNode? MicNode { get; private set; }

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
        Sources.Clear();
        Ports.Clear();
        Links.Clear();
        MicNode = null;
        OutputNode = null;
        PlaybackNode = null;
        HearSounds = MicSounds = MicPassthrough = HearMyself = null;
        await _cli.IsAvailable;
        await Task.Delay(100);
        RefreshObjects(await PipeWireCli.ListObjectsAsync());
        Ports.Sort(PortComparison);
        PhysicalMicrophone ??= Sources.Count == 0 ? null : Sources[0];
        if (PlaybackNode is not null && OutputNode is not null)
            HearSounds = NodeLinkManager.Create(PlaybackNode, OutputNode, Ports, Links);
        if (PlaybackNode is not null && MicNode is not null)
            MicSounds = NodeLinkManager.Create(PlaybackNode, MicNode, Ports, Links);
        if (PhysicalMicrophone is not null && MicNode is not null)
            MicPassthrough = NodeLinkManager.Create(PhysicalMicrophone, MicNode, Ports, Links);
        if (PhysicalMicrophone is not null && OutputNode is not null)
            HearMyself = NodeLinkManager.Create(PhysicalMicrophone, OutputNode, Ports, Links);
    }

    private void RefreshObjects(List<PipeWireObject> objects)
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
                    Links.Add(link);
                    break;
            }
    }

    private async Task RelinkAfterDeviceSwitch()
    {
        var hearSounds = HearSounds?.IsLinked;
        var micSounds = MicSounds?.IsLinked;
        var micPassthrough = MicPassthrough?.IsLinked;
        var hearMyself = HearMyself?.IsLinked;
        await Task.Delay(100);
        await Refresh();
        await Task.WhenAll(
            Relink(HearSounds, hearSounds),
            Relink(MicSounds, micSounds),
            Relink(MicPassthrough, micPassthrough),
            Relink(HearMyself, hearMyself)
        );
    }

    private Task Relink(NodeLinkManager? link, bool? isLinked)
        => link == null || isLinked == null
            ? Task.CompletedTask
            : link.ToggleLink(isLinked, Links);

}
