using System.Threading;
using Avalonia.Threading;
using Soundboword.YouTube;
using YoutubeExplode.Videos;

namespace Soundboword.ViewModels;

public sealed partial class AddFromYouTubeViewModel : ViewModelBase
{

    private readonly SoundList _soundList;

    private CancellationTokenSource? _cts = new();

    public AddFromYouTubeViewModel(SoundList soundList) => _soundList = soundList;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsValid))]
    public partial string SearchQuery { get; set; } = "";

    public bool IsValid => VideoId.TryParse(SearchQuery).HasValue;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsIndeterminate))]
    public partial double Progress { get; private set; } = -1;

    public bool IsIndeterminate => Progress is -1;

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
