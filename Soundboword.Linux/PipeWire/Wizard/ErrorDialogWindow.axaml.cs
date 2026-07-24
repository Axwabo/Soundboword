using Avalonia.Interactivity;

namespace Soundboword.Linux.PipeWire.Wizard;

public sealed partial class ErrorDialogWindow : Window
{

    public static async Task ShowAsync(string error, Window owner) => await new ErrorDialogWindow
    {
        DataContext = new ErrorDialogViewModel {Error = error}
    }.ShowDialog(owner);

    public ErrorDialogWindow() => InitializeComponent();

    private void Button_OnClick(object? sender, RoutedEventArgs e) => Close();

}
