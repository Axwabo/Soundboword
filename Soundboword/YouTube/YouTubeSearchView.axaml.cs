using Avalonia.Interactivity;

namespace Soundboword.YouTube;

public sealed partial class YouTubeSearchView : UserControl
{

    public YouTubeSearchView() => InitializeComponent();

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        Query.Focus();
    }

    private void TextBox_OnPastingFromClipboard(object? sender, RoutedEventArgs e) => (DataContext as YouTubeSearchViewModel)?.OnBeforePaste();

}
