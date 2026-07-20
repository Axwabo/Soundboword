using Soundboword.YouTube;

namespace Soundboword.ViewModels;

public sealed partial class BoardViewModel : PageModelBase
{

    private readonly IServiceProvider? _provider;

    public BoardViewModel() : this(new SoundList(), new EditSoundViewModel(), null)
    {
    }

    public BoardViewModel(SoundList list, EditSoundViewModel editor, IServiceProvider? provider)
    {
        _provider = provider;
        List = list;
        Editor = editor;
    }

    public SoundList List { get; }

    public EditSoundViewModel Editor { get; }

    [ObservableProperty]
    public partial bool DragOver { get; set; }

    [RelayCommand]
    private void StopAll() => List.AudioManager.StopAll();

    [RelayCommand]
    private void AddFromYouTube()
    {
        if (_provider != null)
            AddFromYouTubeWindow.Show(_provider);
    }

    public override void OnActivated()
    {
        if (Editor.Context.Model is { } model)
            Editor.Context.Open(model);
    }

}
