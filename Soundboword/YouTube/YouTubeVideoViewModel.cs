using YoutubeExplode.Videos;

namespace Soundboword.YouTube;

public sealed partial class YouTubeVideoViewModel : ViewModelBase
{

    [ObservableProperty]
    public partial Video? Video { get; private set; }

    public void Open(Video video)
    {
        Video = video;
    }

    public void Close()
    {
        Video = null;
    }

}
