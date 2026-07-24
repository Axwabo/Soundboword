namespace Soundboword.Linux.PipeWire;

[RegisterSingleton<ITabsProvider>]
public sealed class TabsProvider : ITabsProvider
{

    private readonly PipeWireCli _cli;

    private readonly RestartContext _context;
    private readonly TopLevel _topLevel;

    public TabsProvider(PipeWireCli cli, TopLevel topLevel, AudioManager audioManager, DevicesViewModel devices, InputsViewModel inputs, NodeManager nodeManager)
    {
        _cli = cli;
        _topLevel = topLevel;
        _context = new RestartContext(audioManager, devices, inputs, nodeManager);
    }

    public IEnumerable<TabItemViewModel> AdditionalTabs => [new("PipeWire", "🔌", new PipeWireTabViewModel(_cli, _topLevel, _context))];

}
