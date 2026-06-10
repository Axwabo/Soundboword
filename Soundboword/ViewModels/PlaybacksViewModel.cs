using CommunityToolkit.Mvvm.Input;

namespace Soundboword.ViewModels;

public sealed partial class PlaybacksViewModel : ViewModelBase
{

    public AudioManager Manager { get; }

    public PlaybacksViewModel() => Manager = new AudioManager();

    public PlaybacksViewModel(AudioManager audioManager) => Manager = audioManager;

    [RelayCommand]
    private void StopAll() => Manager.StopAll();

}
