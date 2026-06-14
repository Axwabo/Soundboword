using System.Threading;
using Avalonia.Threading;
using Soundboword.YouTube;
using YoutubeExplode.Videos;

namespace Soundboword.ViewModels;

public sealed partial class AddFromYouTubeViewModel : ViewModelBase, IDisposable
{

    private readonly IServiceScope? _serviceScope;

    private readonly SoundList _soundList;

    private CancellationTokenSource? _cts = new();

    public AddFromYouTubeViewModel()
    {
        Search = new YouTubeSearchViewModel();
        Video = new YouTubeVideoViewModel();
    }

    public AddFromYouTubeViewModel(IServiceProvider provider)
    {
        _serviceScope = provider.CreateScope();
        Search = _serviceScope.ServiceProvider.GetRequiredService<YouTubeSearchViewModel>();
        Video = _serviceScope.ServiceProvider.GetRequiredService<YouTubeVideoViewModel>();
    }

    public AddFromYouTubeViewModel(SoundList soundList) => _soundList = soundList;

    public YouTubeSearchViewModel Search { get; }

    public YouTubeVideoViewModel Video { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsValid))]
    public partial string SearchQuery { get; set; } = "";

    public bool IsValid => VideoId.TryParse(SearchQuery).HasValue;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsIndeterminate))]
    public partial double Progress { get; private set; } = -1;

    public bool IsIndeterminate => Progress is -1;

    public void Dispose() => _serviceScope?.Dispose();

    public event Action? Canceled;

    [RelayCommand]
    private async Task Download()
    {
        if (_cts == null)
            return;
        var progress = new Progress<double>(d => Dispatcher.UIThread.Post(() => Progress = d));
        var token = _cts.Token;
        var id = VideoId.Parse(SearchQuery);
        var path = await YouTubeCache.CacheAsync(id, progress, token);
        var sound = new SoundViewModel
        {
            Id = Guid.NewGuid(),
            List = _soundList,
            Name = id,
            Path = path
        };
        _soundList.Sounds.Add(sound);
        _soundList.SaveSounds();
        _soundList.Editor.Open(sound);
        Cancel();
    }

    [RelayCommand]
    public void Cancel()
    {
        if (_cts == null)
            return;
        _cts.Cancel();
        _cts.Dispose();
        _cts = null;
        Canceled?.Invoke();
    }

}
