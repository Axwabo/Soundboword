using Soundboword.Controls;

namespace Soundboword.YouTube;

public sealed partial class AddFromYouTubeWindow : DisposableWindow
{

    public static void Show(IServiceProvider serviceProvider) => new AddFromYouTubeWindow
    {
        DataContext = new AddFromYouTubeViewModel(serviceProvider)
    }.Show();

    public AddFromYouTubeWindow() => InitializeComponent();

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is AddFromYouTubeViewModel {Video: var video})
            video.Completed += Close;
    }

}
