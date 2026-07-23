namespace Soundboword.Services;

public interface ITabsProvider
{

    IEnumerable<TabItemViewModel> GetAdditionalTabs();

}
