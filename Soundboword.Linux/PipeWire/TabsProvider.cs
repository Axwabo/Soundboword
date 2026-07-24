namespace Soundboword.Linux.PipeWire;

[RegisterSingleton<ITabsProvider>]
public sealed class TabsProvider : ITabsProvider
{

    private readonly PipeWireCli _cli;

    public TabsProvider(PipeWireCli cli) => _cli = cli;

    public IEnumerable<TabItemViewModel> GetAdditionalTabs() => [new("PipeWire", "🔌", new PipeWireTabViewModel(_cli))];

}
