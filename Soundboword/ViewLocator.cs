using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Soundboword.ViewModels;

namespace Soundboword;

public sealed class ViewLocator<TView, TViewModel> : IDataTemplate where TView : Control, new() where TViewModel : ViewModelBase
{

    public Control Build(object? param) => new TView();

    public bool Match(object? data) => data is TViewModel;

}
