using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Soundboword.ViewModels;

namespace Soundboword;

public sealed class ViewLocator<TViewModel, TView> : IDataTemplate where TViewModel : ViewModelBase where TView : Control, new()
{

    public Control Build(object? param) => new TView();

    public bool Match(object? data) => data is TViewModel;

}
