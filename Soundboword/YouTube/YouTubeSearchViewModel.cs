using System.Threading;
using Avalonia.Threading;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Search;
using YoutubeExplode.Videos;

namespace Soundboword.YouTube;

public sealed partial class YouTubeSearchViewModel : ViewModelBase, IDisposable
{

    private readonly YouTubeVideoViewModel _videoViewModel;

    private readonly YoutubeClient _youtubeClient;

    private CancellationTokenSource? _cts;

    private bool _isPasting;

    public YouTubeSearchViewModel()
    {
        _youtubeClient = new YoutubeClient();
        _videoViewModel = new YouTubeVideoViewModel(new SoundList());
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

    public YouTubeSearchViewModel(YoutubeClient youtubeClient, YouTubeVideoViewModel videoViewModel)
    {
        _youtubeClient = youtubeClient;
        _videoViewModel = videoViewModel;
    }

    [ObservableProperty]
    public partial string Query { get; set; } = "";

    [ObservableProperty]
    public partial bool IsSearching { get; private set; }

    [ObservableProperty]
    public partial VideoSearchResult? SelectedResult { get; set; }

    public ObservableCollection<VideoSearchResult> Videos { get; } = [];

    public void Dispose()
    {
        _youtubeClient.Dispose();
        _cts?.Dispose();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(Query))
            Search();
        else if (e.PropertyName == nameof(SelectedResult) && SelectedResult is { } result)
        {
            Dispatcher.UIThread.Post(() => SelectedResult = null);
            _videoViewModel.Open(result);
        }
    }

    private void Search()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
        Videos.Clear();
        if (string.IsNullOrEmpty(Query))
        {
            IsSearching = false;
            return;
        }

        if (_isPasting)
        {
            _isPasting = false;
            // TODO: open async
            return;
        }

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
