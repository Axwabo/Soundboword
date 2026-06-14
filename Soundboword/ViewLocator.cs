using Avalonia.Controls.Templates;

namespace Soundboword;

public sealed class ViewLocator<TView, TViewModel> : IDataTemplate where TView : Control, new() where TViewModel : ViewModelBase
{

    public Control Build(object? param) => new TView {DataContext = param};

    public bool Match(object? data) => data is TViewModel;

}
