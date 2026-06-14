using System.Threading;
using Avalonia.Threading;
using YoutubeExplode;
using YoutubeExplode.Videos;

namespace Soundboword.YouTube;

public sealed partial class YouTubeVideoViewModel : ViewModelBase, IDisposable
{

    private readonly YoutubeClient _client;
    private readonly SoundList _soundList;
    private CancellationTokenSource? _details;

    private CancellationTokenSource? _download;

    private VideoId _id;

    public YouTubeVideoViewModel() : this(new YoutubeClient(), new SoundList())
    {
    }

    public YouTubeVideoViewModel(YoutubeClient client, SoundList soundList)
    {
        _client = client;
        _soundList = soundList;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSet), nameof(Id), nameof(Title), nameof(Author), nameof(Description), nameof(Duration), nameof(UploadDate))]
    public partial YouTubeVideo? Video { get; private set; }

    public bool IsSet => Video != null;

    public bool CanDownload => Video != null && !IsDownloading;

    public string Id => $"https://youtu.be/{Video?.Id}";

    public string Title => Video?.Title ?? "Title";

    public string Author => Video?.Author ?? "Author";

    public string Description => Video?.Description ?? "";

    public TimeSpan? Duration => Video?.Duration;

    public DateTimeOffset? UploadDate => Video?.UploadDate;

    [ObservableProperty]
    public partial bool IsLoadingDetails { get; private set; } = true;

    [ObservableProperty]
    public partial bool IsLoadingDescription { get; private set; } = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanDownload))]
    public partial bool IsDownloading { get; private set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsIndeterminate))]
    public partial double Progress { get; private set; } = double.NaN;

    public bool IsIndeterminate => double.IsNaN(Progress);

    public void Dispose()
    {
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
        _id = video.Id;
        Video = YouTubeVideo.Create(video);
        IsLoadingDetails = false;
        IsLoadingDescription = Video.Description == null;
        if (IsLoadingDescription)
            _ = LoadDescriptionAsync();
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
        try
        {
            var token = _download.Token;
            var progress = new Progress<double>(d => Dispatcher.UIThread.Post(() => Progress = d));
            var path = await YouTubeCache.CacheAsync(_id, progress, token);
            _soundList.Add(path, Title);
            _soundList.SaveSounds();
            Completed?.Invoke();
        }
        catch (OperationCanceledException)
        {
        }
        catch
        {
            // TODO: log
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
        _id = id;
        CancelDetails();
        IsLoadingDetails = true;
        IsLoadingDescription = true;
        Video = YouTubeVideo.Loading;
        _details = new CancellationTokenSource();
        var token = _details.Token;
        try
        {
            Open(await _client.Videos.GetAsync(id, token));
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            IsLoadingDetails = IsLoadingDescription = false;
        }
    }

    private async Task LoadDescriptionAsync()
    {
        CancelDetails();
        _details = new CancellationTokenSource();
        var token = _details.Token;
        IsLoadingDescription = true;
        try
        {
            var video = await _client.Videos.GetAsync(_id, token);
            Video = YouTubeVideo.Merge(Video, video);
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            IsLoadingDescription = false;
        }
    }

}
