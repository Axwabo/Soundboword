namespace Soundboword.ViewModels;

public sealed class PlaybacksViewModel : ViewModelBase
{

    public AudioManager Manager { get; }

    public PlaybacksViewModel() => Manager = new AudioManager();

    public PlaybacksViewModel(AudioManager audioManager) => Manager = audioManager;

}
