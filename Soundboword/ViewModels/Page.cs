namespace Soundboword.ViewModels;

public sealed class Page : ViewModelBase
{

    public required Tab Tab { get; init; }

    public required MainWindowViewModel Parent { get; init; }

    public string Header => Tab.Header;

    public string Icon => Tab.Icon;

    public ViewModelBase Content => Tab.Content;

}
