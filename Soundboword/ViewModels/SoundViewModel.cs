using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Soundboword.Models;
using Soundboword.Services;

namespace Soundboword.ViewModels;

public sealed partial class SoundViewModel : ViewModelBase
{

    public required SoundId Id { get; init; }

    public required SoundList List { get; init; }

    public required string Path { get; set; }

    [ObservableProperty]
    public required partial string Name { get; set; }

    [ObservableProperty]
    public partial TriggerMode Mode { get; set; }

    [ObservableProperty]
    public partial bool Loop { get; set; }

    [ObservableProperty]
    public partial float Volume { get; set; } = 1;

    [ObservableProperty]
    public partial SoundState PlaybackState { get; private set; }

    [RelayCommand]
    private void Trigger() => List.AudioManager.Trigger(this);

    [RelayCommand]
    private void Stop() => List.AudioManager.StopAll(this);

    [RelayCommand]
    private void ToggleLoop() => Loop = !Loop;

    [RelayCommand]
    private void Configure() => List.Editor.Open(this);

    public void UpdatePlaybackState(SoundState state)
    {
        if (Dispatcher.UIThread.CheckAccess())
            PlaybackState = state;
        else
            Dispatcher.UIThread.Post(() => PlaybackState = state);
    }

}
