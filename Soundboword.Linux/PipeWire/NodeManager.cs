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
        devicesViewModel.DeviceSwitched = RefreshPersist;
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

    public Task RefreshPersist()
    {
        if (_relinkTask is not {IsCompleted: false})
            _relinkTask = RelinkAfterDeviceSwitch();
        return _relinkTask;
    }

    public async Task Refresh() => await RefreshAsync(null, null, null, null);

    private async Task RefreshAsync(bool? hearSounds, bool? micSounds, bool? micPassthrough, bool? hearMyself)
    {
        Sources.Clear();
        Ports.Clear();
        Links.Clear();
        MicNode = null;
        OutputNode = null;
        PlaybackNode = null;
        await _cli.IsAvailable;
        await Task.Delay(100);
        RefreshObjects(await PipeWireCli.ListObjectsAsync());
        Ports.Sort(PortComparison);
        PhysicalMicrophone ??= Sources.Count == 0 ? null : Sources[0];
        Task linkMicSounds, linkMicPassthrough, linkHearMyself;
        var linkHearSounds = linkMicSounds = linkMicPassthrough = linkHearMyself = Task.CompletedTask;
        if (PlaybackNode is not null && OutputNode is not null)
            (HearSounds, linkHearSounds) = NodeLinkManager.Create(PlaybackNode, OutputNode, Ports, Links, hearSounds);
        else
            HearSounds = null;
        if (PlaybackNode is not null && MicNode is not null)
            (MicSounds, linkMicSounds) = NodeLinkManager.Create(PlaybackNode, MicNode, Ports, Links, micSounds);
        else
            MicSounds = null;
        if (PhysicalMicrophone is not null && MicNode is not null)
            (MicPassthrough, linkMicPassthrough) = NodeLinkManager.Create(PhysicalMicrophone, MicNode, Ports, Links, micPassthrough);
        else
            MicPassthrough = null;
        if (PhysicalMicrophone is not null && OutputNode is not null)
            (HearMyself, linkHearMyself) = NodeLinkManager.Create(PhysicalMicrophone, OutputNode, Ports, Links, hearMyself);
        else
            HearMyself = null;
        await Task.WhenAll(linkHearSounds, linkMicSounds, linkMicPassthrough, linkHearMyself);
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
        await Task.Delay(200);
        var output = OutputNode;
        var hearMyself = HearMyself;
        await RefreshAsync(HearSounds?.IsLinked, MicSounds?.IsLinked, MicPassthrough?.IsLinked, HearMyself?.IsLinked);
        if (output != OutputNode && hearMyself is {IsLinked: true})
            await hearMyself.ToggleLink(false, Links);
    }

}
