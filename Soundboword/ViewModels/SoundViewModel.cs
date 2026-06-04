using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Soundboword.Models;
using Soundboword.Services;

namespace Soundboword.ViewModels;

public sealed partial class SoundViewModel : ViewModelBase
{

    public required string Path { get; init; }

    public required SoundList List { get; init; }

    public required IFileManagerOpener? Opener { get; init; }

    [ObservableProperty]
    public required partial string Name { get; set; }

    [ObservableProperty]
    public partial PlaybackMode Mode { get; set; }

    [ObservableProperty]
    public partial bool Loop { get; set; }

    [ObservableProperty]
    public partial bool IsPlaying { get; private set; }

    [RelayCommand]
    private void Trigger() => AudioManager.Trigger(this);

    [RelayCommand]
    private void Reveal() => Opener?.Open(Path);

    public void UpdatePlaybackState(bool active)
    {
        if (Dispatcher.UIThread.CheckAccess())
            IsPlaying = active;
        else
            Dispatcher.UIThread.Post(() => IsPlaying = active);
    }

}
