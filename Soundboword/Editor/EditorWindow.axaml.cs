using Soundboword.Controls;

namespace Soundboword.Editor;

public sealed partial class EditorWindow : DisposableWindow
{

    public static EditorWindow Show(EditorList list,IServiceProvider provider, SoundViewModel sound)
    {
        var window = new EditorWindow
        {
            DataContext = new EditorWindowViewModel(provider, sound),
            _list = list,
            _id = sound.Id
        };
        window.Show();
        return window;
    }

    private SoundId _id;

    private EditorList? _list;

    public EditorWindow() => InitializeComponent();

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        var id = _id;
        _id = default;
        _list?.Close(id);
    }

}

