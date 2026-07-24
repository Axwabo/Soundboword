namespace Soundboword.Linux.PipeWire;

[RegisterSingleton<ITabsProvider>]
public sealed class TabsProvider : ITabsProvider
{

    private readonly AudioManager _audioManager;

    private readonly PipeWireCli _cli;
    private readonly DevicesViewModel _devices;
    private readonly TopLevel _topLevel;

    public TabsProvider(PipeWireCli cli, TopLevel topLevel, AudioManager audioManager, DevicesViewModel devices)
    {
        _cli = cli;
        _topLevel = topLevel;
        _audioManager = audioManager;
        _devices = devices;
    }

    public IEnumerable<TabItemViewModel> AdditionalTabs => [new("PipeWire", "🔌", new PipeWireTabViewModel(_cli, _topLevel, _audioManager, _devices))];

}
