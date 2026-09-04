using Avalonia.Input;

namespace Soundboword.Views;

public sealed partial class MainWindow : Window
{

    public MainWindow() => InitializeComponent();

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is MainWindowViewModel {Toggles: null})
            ContentGrid.RowDefinitions = new RowDefinitions("*");
    }

    private void NavigateToLogs(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is MainWindowViewModel model)
            model.CurrentPage = model.LogsPage;
    }

}
