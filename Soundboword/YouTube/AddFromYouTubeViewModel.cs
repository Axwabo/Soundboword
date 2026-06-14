namespace Soundboword.YouTube;

public sealed class AddFromYouTubeViewModel : ViewModelBase, IDisposable
{

    private readonly IServiceScope? _serviceScope;

    public AddFromYouTubeViewModel()
    {
        Search = new YouTubeSearchViewModel();
        Video = new YouTubeVideoViewModel(new SoundList());
    }

    public AddFromYouTubeViewModel(IServiceProvider provider)
    {
        _serviceScope = provider.CreateScope();
        Search = _serviceScope.ServiceProvider.GetRequiredService<YouTubeSearchViewModel>();
        Video = _serviceScope.ServiceProvider.GetRequiredService<YouTubeVideoViewModel>();
    }

    public YouTubeSearchViewModel Search { get; }

    public YouTubeVideoViewModel Video { get; }

    public void Dispose() => _serviceScope?.Dispose();

}
