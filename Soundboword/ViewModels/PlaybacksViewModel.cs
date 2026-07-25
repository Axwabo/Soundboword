namespace Soundboword.ViewModels;

public sealed partial class PlaybacksViewModel : ViewModelBase
{

    public PlaybacksViewModel() => Manager = new AudioManager(new SoundFlowDeviceManager());

    public PlaybacksViewModel(AudioManager audioManager) => Manager = audioManager;

    public AudioManager Manager { get; }

    [RelayCommand]
    private void StopAll() => Manager.StopAll();

}
