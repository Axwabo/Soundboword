namespace Soundboword.Linux.PipeWire;

[RegisterSingleton<TabListToggles>]
public sealed partial class LinkTogglesViewModel : TabListToggles
{

    private readonly NodeManager _manager;

    public LinkTogglesViewModel(NodeManager manager) => _manager = manager;

    [ObservableProperty]
    public partial bool HearMyself { get; private set; }

    [ObservableProperty]
    public partial bool TogglingHearMyself { get; private set; }

}
