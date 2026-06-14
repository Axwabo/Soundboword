namespace Soundboword.Views;

public sealed partial class AddFromYouTubeWindow : Window
{

    public AddFromYouTubeWindow() => InitializeComponent();

    public static void Show(IServiceProvider serviceProvider) => new AddFromYouTubeWindow
    {
        DataContext = new AddFromYouTubeViewModel(serviceProvider)
    }.Show();

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        (DataContext as IDisposable)?.Dispose();
    }

}
