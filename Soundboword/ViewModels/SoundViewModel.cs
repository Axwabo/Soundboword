using System;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Soundboword.Models;
using Soundboword.Services;

namespace Soundboword.ViewModels;

public sealed partial class SoundViewModel : ViewModelBase
{

    public required Guid Id { get; init; }

    public required string Path { get; init; }

    public required SoundList List { get; init; }

    [ObservableProperty]
    public required partial string Name { get; set; }

    [ObservableProperty]
    public partial TriggerMode Mode { get; set; }

    [ObservableProperty]
    public partial bool Loop { get; set; }

    [ObservableProperty]
    public partial bool IsPlaying { get; private set; }

    [RelayCommand]
    private void Trigger() => AudioManager.Trigger(this);

    [RelayCommand]
    private void Stop() => AudioManager.StopAll(this);

    [RelayCommand]
    private void ToggleLoop() => Loop = !Loop;

    [RelayCommand]
    private void Configure() => List.Editor.Open(this);

    public void UpdatePlaybackState(bool active)
    {
        if (Dispatcher.UIThread.CheckAccess())
            IsPlaying = active;
        else
            Dispatcher.UIThread.Post(() => IsPlaying = active);
    }

}
