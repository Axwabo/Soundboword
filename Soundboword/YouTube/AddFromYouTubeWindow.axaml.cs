namespace Soundboword.YouTube;

public sealed partial class AddFromYouTubeWindow : Window
{

    public AddFromYouTubeWindow() => InitializeComponent();

    public static void Show(IServiceProvider serviceProvider) => new AddFromYouTubeWindow
    {
        DataContext = new AddFromYouTubeViewModel(serviceProvider)
    }.Show();

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is AddFromYouTubeViewModel {Video: var video})
            video.Completed += Close;
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        (DataContext as IDisposable)?.Dispose();
    }

}
