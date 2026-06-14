using System.Threading;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Search;
using YoutubeExplode.Videos;

namespace Soundboword.YouTube;

public sealed partial class YouTubeSearchViewModel : ViewModelBase, IDisposable
{

    private readonly YoutubeClient _youtubeClient;

    private CancellationTokenSource? _cts;

    private bool _isPasting; // TODO: auto-open video view

    public YouTubeSearchViewModel() : this(new YoutubeClient())
    {
        var videoSearchResult = new VideoSearchResult(new VideoId(), "Among us in real life", new Author(default, "Sussy baka"), TimeSpan.FromMinutes(3), []);
        Videos.Add(videoSearchResult);
        Videos.Add(videoSearchResult);
        Videos.Add(videoSearchResult);
        Videos.Add(videoSearchResult);
        var searchResult = new VideoSearchResult(new VideoId(), "real", new Author(default, "fake"), new TimeSpan(1, 2, 3), []);
        Videos.Add(searchResult);
        Videos.Add(searchResult);
        Videos.Add(searchResult);
        Videos.Add(searchResult);
        Videos.Add(searchResult);
        Videos.Add(searchResult);
    }

    public YouTubeSearchViewModel(YoutubeClient youtubeClient) => _youtubeClient = youtubeClient;

    [ObservableProperty]
    public partial string Query { get; set; } = "";

    [ObservableProperty]
    public partial bool IsSearching { get; private set; }

    public ObservableCollection<VideoSearchResult> Videos { get; } = [];

    public void Dispose()
    {
        _youtubeClient.Dispose();
        _cts?.Dispose();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName != nameof(Query))
            return;
        _cts?.Cancel();
        _cts?.Dispose();
        Videos.Clear();
        _cts = new CancellationTokenSource();
        _ = SearchAsync(Query, _cts.Token);
    }

    // TODO: error handling
    private async Task SearchAsync(string query, CancellationToken cancellationToken)
    {
        IsSearching = true;
        await foreach (var result in _youtubeClient.Search.GetVideosAsync(query, cancellationToken))
        {
            if (result.Duration == null)
                continue;
            Videos.Add(result);
            if (Videos.Count >= 20)
                break;
        }

        IsSearching = false;
    }

    public void OnBeforePaste() => _isPasting = true;

}
