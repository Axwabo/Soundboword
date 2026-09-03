namespace Soundboword.Editor;

[RegisterSingleton]
public sealed class EditorList
{

    private readonly Dictionary<SoundId, EditorWindow> _editors = [];

    private readonly IServiceProvider _provider;

    public EditorList(IServiceProvider provider) => _provider = provider;

    public void Open(SoundViewModel sound)
    {
        if (_editors.TryGetValue(sound.Id, out var existing))
            existing.Activate();
        else
            _editors[sound.Id] = EditorWindow.Show(this, _provider, sound);
    }

    public void Close(SoundId sound)
    {
        if(_editors.Remove(sound, out var window))
            window.Close();
    }

}
