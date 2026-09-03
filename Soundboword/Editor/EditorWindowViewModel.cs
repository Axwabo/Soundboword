namespace Soundboword.Editor;

public sealed class EditorWindowViewModel : ViewModelBase
{

    private readonly IServiceScope? _serviceScope;

    public EditorWindowViewModel() => Content = new EditorContentViewModel(new EditorContext
    {
        Sound = new SoundViewModel
        {
            Id = Guid.CreateVersion7(),
            List = null!,
            Name = "amogus",
            Path = "/sus/amogus.wav",
            Duration = TimeSpan.FromSeconds(5)
        }
    });

    public EditorWindowViewModel(IServiceProvider provider, SoundViewModel sound)
    {
        _serviceScope = provider.CreateScope();
        _serviceScope.ServiceProvider.GetRequiredService<EditorContext>().Sound = sound;
        Content = _serviceScope.ServiceProvider.GetRequiredService<EditorContentViewModel>();
    }

    public EditorContentViewModel Content { get; }

    public void Dispose() => _serviceScope?.Dispose();

}
