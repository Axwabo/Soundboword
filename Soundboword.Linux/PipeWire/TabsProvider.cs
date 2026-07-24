namespace Soundboword.Linux.PipeWire;

[RegisterSingleton<ITabsProvider>]
public sealed class TabsProvider : ITabsProvider
{

    private readonly PipeWireCli _cli;
    private readonly TopLevel _topLevel;

    public TabsProvider(PipeWireCli cli, TopLevel topLevel)
    {
        _cli = cli;
        _topLevel = topLevel;
    }

    public IEnumerable<TabItemViewModel> AdditionalTabs => [new("PipeWire", "🔌", new PipeWireTabViewModel(_cli, _topLevel))];

}
