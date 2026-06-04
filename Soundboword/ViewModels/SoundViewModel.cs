using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Soundboword.Models;

namespace Soundboword.ViewModels;

public sealed partial class SoundViewModel : ViewModelBase
{

    public required string Path { get; init; }

    [ObservableProperty]
    public required partial string Name { get; set; }

    [ObservableProperty]
    public partial PlaybackMode Mode { get; set; }

    [RelayCommand]
    private void Play() => AudioManager.Play(this);

}
