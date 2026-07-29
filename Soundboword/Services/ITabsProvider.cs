namespace Soundboword.Services;

public interface ITabsProvider
{

    IEnumerable<Tab> AdditionalTabs { get; }

}
