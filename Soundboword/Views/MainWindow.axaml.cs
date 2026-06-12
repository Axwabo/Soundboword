using Avalonia.Controls;
using Soundboword.ViewModels;

namespace Soundboword.Views;

public sealed partial class MainWindow : Window
{

    public MainWindow() => InitializeComponent();

    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.Source is not TabControl control || DataContext is not MainWindowViewModel model)
            return;
        model.ShortcutAssigner.Close();
        if (control.SelectedValue is TabItem {Content: UserControl {Content: PageModelBase page}})
            page.OnActivated();
    }

}
