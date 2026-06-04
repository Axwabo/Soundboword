using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Soundboword.Models;
using Soundboword.Services;

namespace Soundboword.ViewModels;

public sealed partial class SoundViewModel : ViewModelBase
{

    public required string Path { get; init; }

    public required IFileManagerOpener? Opener { get; init; }

    [ObservableProperty]
    public required partial string Name { get; set; }

    [ObservableProperty]
    public partial PlaybackMode Mode { get; set; }

    [ObservableProperty]
    public partial bool Loop { get; set; }

    [RelayCommand]
    private void Play() => AudioManager.Trigger(this);

    [RelayCommand]
    private void Reveal() => Opener?.Open(Path);

}
