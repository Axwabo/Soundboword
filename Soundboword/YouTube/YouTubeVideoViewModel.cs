using System.Threading;
using Avalonia.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.ClosedCaptions;
using YoutubeExplode.Videos.Streams;
using Container = YoutubeExplode.Videos.Streams.Container;

namespace Soundboword.YouTube;

public sealed partial class YouTubeVideoViewModel : ViewModelBase, IDisposable
{

    private readonly YouTubeCache _cache;

    private readonly YoutubeClient _client;

    private readonly ILogger _logger;
    private readonly SoundList _soundList;

    private CancellationTokenSource? _details;

    private CancellationTokenSource? _download;

    public YouTubeVideoViewModel()
    {
        _client = new YoutubeClient();
        _cache = new YouTubeCache(_client);
        _soundList = new SoundList();
        _logger = NullLogger.Instance;
        Streams.Add(new AudioOnlyStreamInfo("", Container.Mp3, new FileSize(10000), new Bitrate(10000), "aac", null, null));
        Streams.Add(new AudioOnlyStreamInfo("", Container.WebM, new FileSize(3000), new Bitrate(8000), "idk", new Language("en-US", "English madafaka"), null));
    }

    public YouTubeVideoViewModel(YoutubeClient client, YouTubeCache cache, SoundList soundList, ILoggerFactory loggerFactory)
    {
        _client = client;
        _cache = cache;
        _soundList = soundList;
        _logger = loggerFactory.CreateLogger("YouTube Downloader");
    }

    [ObservableProperty]
    public partial VideoId Id { get; private set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSet), nameof(Id), nameof(Title), nameof(Author), nameof(Duration), nameof(UploadDate))]
    public partial YouTubeVideo? Video { get; private set; }

    public bool IsSet => Video != null;

    public bool CanDownload => Video != null && !IsDownloading;

    public string Title => Video?.Title ?? "Title";

    public string Author => Video?.Author ?? "Author";

    public TimeSpan Duration => Video?.Duration ?? TimeSpan.Zero;

    public DateTimeOffset UploadDate => Video?.UploadDate ?? DateTimeOffset.UnixEpoch;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    public partial string? Error { get; private set; }

    public bool HasError => Error != null;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLoading))]
    public partial bool IsLoadingDetails { get; private set; } = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLoading))]
    public partial bool IsLoadingStreams { get; private set; }

    public bool IsLoading => IsLoadingDetails || IsLoadingStreams;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanDownload))]
    public partial bool IsDownloading { get; private set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsIndeterminate))]
    public partial double Progress { get; private set; } = double.NaN;

    public bool IsIndeterminate => double.IsNaN(Progress);

    public ObservableCollection<AudioOnlyStreamInfo> Streams { get; } = [];

    [ObservableProperty]
    public partial AudioOnlyStreamInfo? SelectedStream { get; set; }

    [ObservableProperty]
    public partial bool Mp3 { get; set; } = true;

    [ObservableProperty]
    public partial bool Wav { get; set; }

    public void Dispose()
    {
        Error = null;
        SelectedStream = null;
        Streams.Clear();
        CancelDetails();
        CancelDownload();
    }

    private void CancelDetails()
    {
        _details?.Cancel();
        _details?.Dispose();
        _details = null;
    }

    public event Action? Completed;

    public void Open(IVideo video)
    {
        Id = video.Id;
        Video = YouTubeVideo.Create(video);
        IsLoadingDetails = false;
        if (IsLoadingStreams)
            return;
        IsLoadingStreams = Streams.Count == 0;
        if (IsLoadingStreams)
            _ = LoadStreamsAsync();
    }

    [RelayCommand]
    public void Close()
    {
        Dispose();
        Video = null;
    }

    [RelayCommand]
    private async Task DownloadAsync()
    {
        if (_download != null)
            return;
        _download = new CancellationTokenSource();
        IsDownloading = true;
        var title = Title;
        try
        {
            var token = _download.Token;
            var progress = new Progress<double>(d => Dispatcher.UIThread.Post(() => Progress = d));
            var path = await _cache.CacheAsync(Id, SelectedStream, progress, Wav ? YouTubeCache.Wav : Container.Mp3, token);
            _soundList.Add(path, title);
            _soundList.SaveSounds();
            Completed?.Invoke();
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            LogFailedDownload(title, e);
        }
        finally
        {
            IsDownloading = false;
        }
    }

    [RelayCommand]
    private void CancelDownload()
    {
        _download?.Cancel();
        _download?.Dispose();
        _download = null;
        IsDownloading = false;
        Progress = double.NaN;
    }

    public async Task OpenAsync(VideoId id)
    {
        Id = id;
        IsLoadingDetails = true;
        IsLoadingStreams = true;
        Video = YouTubeVideo.Loading;
        try
        {
            _ = LoadStreamsAsync();
            Open(await _client.Videos.GetAsync(id, _details!.Token));
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            Error = $"Failed to load video metadata\n{e.Message}";
            LogFailedFetch(id, e);
        }
        finally
        {
            IsLoadingDetails = false;
        }
    }

    private async Task LoadStreamsAsync()
    {
        CancelDetails();
        _details = new CancellationTokenSource();
        var token = _details.Token;
        IsLoadingStreams = true;
        try
        {
            var manifest = await _client.Videos.Streams.GetManifestAsync(Id, token);
            foreach (var info in manifest.GetAudioOnlyStreams())
                Streams.Add(info);
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            IsLoadingStreams = false;
        }
    }

    [LoggerMessage(LogLevel.Information, "Failed to fetch information for video {Id}")]
    private partial void LogFailedFetch(VideoId id, Exception exception);

    [LoggerMessage(LogLevel.Error, "Failed to download {Title}")]
    private partial void LogFailedDownload(string title, Exception exception);

}
