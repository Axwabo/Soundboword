using System.Threading;
using Avalonia.Threading;
using YoutubeExplode;
using YoutubeExplode.Videos;

namespace Soundboword.YouTube;

public sealed partial class YouTubeVideoViewModel : ViewModelBase, IDisposable
{

    private readonly YoutubeClient _client;
    private readonly SoundList _soundList;

    private CancellationTokenSource? _cts;

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
    [NotifyPropertyChangedFor(nameof(IsSet))]
    public partial YouTubeVideo? Video { get; private set; }

    public bool IsSet => Video != null;

    [ObservableProperty]
    public partial string Id { get; private set; } = "";

    [ObservableProperty]
    public partial string Title { get; private set; } = "";

    [ObservableProperty]
    public partial bool IsDownloading { get; private set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsIndeterminate))]
    public partial double Progress { get; private set; } = double.NaN;

    public bool IsIndeterminate => double.IsNaN(Progress);

    public void Dispose() => Cancel();

    public event Action? Completed;

    public void Open(YouTubeVideo video)
    {
        _id = video.Id;
        Video = video;
        Id = $"https://youtu.be/{video.Id}";
        Title = video.Title;
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
        if (_cts != null)
            return;
        _cts = new CancellationTokenSource();
        IsDownloading = true;
        try
        {
            var token = _cts.Token;
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
    private void Cancel()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
        IsDownloading = false;
        Progress = double.NaN;
    }

}
