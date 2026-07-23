namespace Soundboword.Linux;

[RegisterSingleton<ITabsProvider>]
public sealed class TabsProvider : ITabsProvider
{

    public IEnumerable<TabItemViewModel> GetAdditionalTabs() => [new("PipeWire", "🔌", null!)];

}
