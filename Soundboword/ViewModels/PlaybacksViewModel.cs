using CommunityToolkit.Mvvm.Input;
using Soundboword.Services;

namespace Soundboword.ViewModels;

public sealed partial class PlaybacksViewModel : ViewModelBase
{

    public AudioManager Manager { get; }

    public PlaybacksViewModel() => Manager = new AudioManager(new SoundFlowDeviceManager());

    public PlaybacksViewModel(AudioManager audioManager) => Manager = audioManager;

    [RelayCommand]
    private void StopAll() => Manager.StopAll();

}
