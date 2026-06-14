using Avalonia.Threading;

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
    [NotifyPropertyChangedFor(nameof(AnyPlaybacks), nameof(CanTrigger), nameof(CanRelink))]
    public partial SoundState PlaybackState { get; private set; }

    public bool AnyPlaybacks => PlaybackState is SoundState.Playing or SoundState.Paused;

    public bool CanTrigger => PlaybackState != SoundState.Error;

    public bool CanRelink => PlaybackState is SoundState.Stopped or SoundState.Error;

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
