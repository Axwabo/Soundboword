using Soundboword.Linux.PipeWire.Settings;
using Soundboword.Settings;

namespace Soundboword.Linux.PipeWire;

[RegisterSingleton(Registration = RegistrationStrategy.Self)]
public sealed partial class NodeManager : ObservableObject
{

    private const string Stream = "Stream/Output/Audio";

    private static readonly Comparison<PipeWirePort> PortComparison = (a, b) => a.PortId.CompareTo(b.PortId);

    private readonly PipeWireCli _cli;
    private readonly SoundFlowDeviceManager _outputManager;
    private readonly PipeWirePreferences _preferences;

    public NodeManager(PipeWireCli cli, SoundFlowDeviceManager outputManager, SettingsManager settingsManager)
    {
        _cli = cli;
        _outputManager = outputManager;
        _preferences = settingsManager.Require<PipeWirePreferences>();
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

    public async Task Refresh() => await RefreshAsync(null, _preferences.AutoMicSounds, null, null);

    public async Task RefreshAsync(bool? hearSounds, bool? micSounds, bool? micPassthrough, bool? hearMyself)
    {
        Sources.Clear();
        Ports.Clear();
        Links.Clear();
        await _cli.IsAvailable;
        await Task.Delay(100);
        RefreshObjects(await PipeWireCli.ListObjectsAsync());
        Ports.Sort(PortComparison);
        PhysicalMicrophone ??= Sources.Count == 0 ? null : Sources[0];
        (HearSounds, var linkHearSounds) = NodeLinkManager.Create(PlaybackNode, OutputNode, Ports, Links, hearSounds);
        (MicSounds, var linkMicSounds) = NodeLinkManager.Create(PlaybackNode, MicNode, Ports, Links, micSounds);
        (MicPassthrough, var linkMicPassthrough) = NodeLinkManager.Create(PhysicalMicrophone, MicNode, Ports, Links, micPassthrough);
        (HearMyself, var linkHearMyself) = NodeLinkManager.Create(PhysicalMicrophone, OutputNode, Ports, Links, hearMyself);
        await Task.WhenAll(linkHearSounds, linkMicSounds, linkMicPassthrough, linkHearMyself);
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

}
