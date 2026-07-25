namespace Soundboword.Models;

public abstract class TabListToggles : ViewModelBase
{

    public abstract IAsyncRelayCommand? GetCommand(string id);

}
