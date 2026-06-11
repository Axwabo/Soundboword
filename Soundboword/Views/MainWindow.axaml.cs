using Avalonia.Controls;
using Soundboword.ViewModels;

namespace Soundboword.Views;

public sealed partial class MainWindow : Window
{

    public MainWindow() => InitializeComponent();

    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is MainWindowViewModel model)
            model.ShortcutAssigner.Close();
    }

}
