using Avalonia.Input;

namespace Soundboword.Views;

public sealed partial class MainWindow : Window
{

    public MainWindow() => InitializeComponent();

    private void NavigateToLogs(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is MainWindowViewModel model)
            model.CurrentPage = model.LogsPage;
    }

}
