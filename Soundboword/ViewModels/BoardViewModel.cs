using CommunityToolkit.Mvvm.Input;
using Soundboword.Services;

namespace Soundboword.ViewModels;

public sealed partial class BoardViewModel : ViewModelBase
{

    public SoundList List { get; }

    public EditSoundViewModel Editor { get; }

    public BoardViewModel() : this(new SoundList(), new EditSoundViewModel())
    {
    }

    public BoardViewModel(SoundList list, EditSoundViewModel editor)
    {
        List = list;
        Editor = editor;
    }

    [RelayCommand]
    private static void StopAll() => AudioManager.StopAll();

}
