namespace Soundboword.Services;

public interface ITabsProvider
{

    IEnumerable<TabItemViewModel> AdditionalTabs { get; }

}
