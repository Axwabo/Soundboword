using CommunityToolkit.Mvvm.ComponentModel;

namespace Soundboword.ViewModels;

public abstract class ViewModelBase : ObservableObject;

public abstract class PageModelBase : ViewModelBase
{

    public virtual void OnActivated()
    {
    }

}
