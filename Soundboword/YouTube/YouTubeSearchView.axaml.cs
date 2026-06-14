using Avalonia.Interactivity;

namespace Soundboword.YouTube;

public partial class YouTubeSearchView : UserControl
{

    public YouTubeSearchView() => InitializeComponent();

    private void TextBox_OnPastingFromClipboard(object? sender, RoutedEventArgs e)
    {
        (DataContext as YouTubeSearchViewModel)?.OnBeforePaste();
    }

}
