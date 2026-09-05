using Soundboword.Linux.PipeWire.Settings;

namespace Soundboword.Linux.PipeWire;

[RegisterSingleton(Registration = RegistrationStrategy.Self)]
public sealed partial class NodeManager : ObservableObject
{

    private const string Stream = "Stream/Output/Audio";
    private const string FileName = "micorphone";

    private static readonly Comparison<PipeWirePort> PortComparison = (a, b) => a.PortId.CompareTo(b.PortId);

    private readonly PipeWireCli _cli;
    private readonly UserData _data;
    private readonly SoundFlowDeviceManager _outputManager;

    private bool _isRefreshing;

    private string? _sourceName;

    public NodeManager(PipeWireCli cli, SoundFlowDeviceManager outputManager, [FromKeyedServices(PipeWirePreferences.Key)] UserData data)
    {
        _cli = cli;
        _outputManager = outputManager;
        _data = data;
    }

    public ObservableCollection<PipeWireNode> Sources { get; } = [];

    private List<PipeWirePort> Ports { get; } = [];

    public List<PipeWireLink> Links { get; } = [];

    public PipeWireNode? OutputNode { get; set; }

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

    public async Task RefreshAsync(bool? hearSounds, bool? micSounds, bool? micPassthrough, bool? hearMyself)
    {
        _isRefreshing = true;
        var preferred = _data.Load(FileName);
        _sourceName = PhysicalMicrophone?.Description;
        Sources.Clear();
        Ports.Clear();
        Links.Clear();
        await _cli.IsAvailable;
        await Task.Delay(100);
        RefreshObjects(await PipeWireCli.ListObjectsAsync());
        Ports.Sort(PortComparison);
        PhysicalMicrophone = Sources.Count == 0
            ? null
            : GetSource(_sourceName) ?? GetSource(preferred) ?? Sources[0];
        (HearSounds, var linkHearSounds) = NodeLinkManager.Create(PlaybackNode, OutputNode, Ports, Links, hearSounds);
        (MicSounds, var linkMicSounds) = NodeLinkManager.Create(PlaybackNode, MicNode, Ports, Links, micSounds);
        var (linkMicPassthrough, linkHearMyself) = UpdatePhysicalMic(micPassthrough, hearMyself);
        await Task.WhenAll(linkHearSounds, linkMicSounds, linkMicPassthrough, linkHearMyself);
        _isRefreshing = false;
    }

    private PipeWireNode? GetSource(string? preferred)
        => preferred == null
            ? null
            : Sources.FirstOrDefault(e => e.Description == preferred);

    public (Task, Task) UpdatePhysicalMic(bool? micPassthrough, bool? hearMyself)
    {
        (MicPassthrough, var linkMicPassthrough) = NodeLinkManager.Create(PhysicalMicrophone, MicNode, Ports, Links, micPassthrough);
        (HearMyself, var linkHearMyself) = NodeLinkManager.Create(PhysicalMicrophone, OutputNode, Ports, Links, hearMyself);
        return (linkMicPassthrough, linkHearMyself);
    }

    private void RefreshObjects(List<PipeWireObject> objects)
    {
        MicNode = null;
        OutputNode = null;
        PlaybackNode = null;
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

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName != nameof(PhysicalMicrophone))
            return;
        _outputManager.InvokeMicrophoneSwitched();
        if (!_isRefreshing && PhysicalMicrophone is {Description: { } name})
            _data.Save(FileName, name);
    }

}
