namespace Soundboword.Editor;

public sealed class EditorWindowViewModel : ViewModelBase
{

    private readonly IServiceScope? _serviceScope;

    public EditorWindowViewModel() => Content = new EditorContentViewModel(new EditorContext());

    public EditorWindowViewModel(IServiceProvider provider, SoundViewModel sound)
    {
        _serviceScope = provider.CreateScope();
        _serviceScope.ServiceProvider.GetRequiredService<EditorContext>().Sound = sound;
        Content = _serviceScope.ServiceProvider.GetRequiredService<EditorContentViewModel>();
    }

    public EditorContentViewModel Content { get; }

    public void Dispose() => _serviceScope?.Dispose();

}
