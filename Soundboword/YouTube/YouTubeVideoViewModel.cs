using YoutubeExplode.Search;

namespace Soundboword.YouTube;

public sealed partial class YouTubeVideoViewModel : ViewModelBase
{

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSet))]
    public partial VideoSearchResult? Video { get; private set; }

    public bool IsSet => Video != null;

    [ObservableProperty]
    public partial string Id { get; private set; } = "";

    [ObservableProperty]
    public partial string Title { get; private set; } = "";

    public void Open(VideoSearchResult video)
    {
        Video = video;
        Id = $"https://youtu.be/{video.Id}";
        Title = video.Title;
    }

    [RelayCommand]
    public void Close()
    {
        Video = null;
    }

    [RelayCommand]
    private async Task DownloadAsync()
    {
    }

}
