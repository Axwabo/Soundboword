namespace Soundboword.ViewModels;

public sealed partial class BoardViewModel : PageModelBase
{

    public BoardViewModel() : this(new SoundList(), new EditSoundViewModel())
    {
    }

    public BoardViewModel(SoundList list, EditSoundViewModel editor)
    {
        List = list;
        Editor = editor;
    }

    public SoundList List { get; }

    public EditSoundViewModel Editor { get; }

    [RelayCommand]
    private void StopAll() => List.AudioManager.StopAll();

    public override void OnActivated()
    {
        if (Editor.Context.Model is { } model)
            Editor.Context.Open(model);
    }

}
