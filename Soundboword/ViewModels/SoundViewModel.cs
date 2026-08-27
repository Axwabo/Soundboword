using Avalonia.Threading;
using SoundFlow.Metadata;
using SoundFlow.Metadata.Models;

namespace Soundboword.ViewModels;

public sealed partial class SoundViewModel : ViewModelBase, IPlaybackSuspender
{

    private static readonly ReadOptions ReadOptions = new()
    {
        DurationAccuracy = DurationAccuracy.FastEstimate,
        ReadTags = false
    };

    private readonly HashSet<IPlaybackSuspender> _suspenders = [];

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
    public partial OtherSoundInteraction Interaction { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AnyPlaybacks), nameof(CanTrigger), nameof(CanRelink), nameof(IsNotFound))]
    public partial SoundState PlaybackState { get; private set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Information))]
    public partial TimeSpan? Duration { get; set; }

    public bool AnyPlaybacks => PlaybackState is SoundState.Playing or SoundState.Paused;

    public bool CanTrigger => PlaybackState != SoundState.NotFound;

    public bool CanRelink => PlaybackState is SoundState.Stopped or SoundState.NotFound;

    public bool IsNotFound => PlaybackState == SoundState.NotFound;

    public bool IsPaused
    {
        get
        {
            lock (_suspenders)
            {
                return _suspenders.Count != 0;
            }
        }
    }

    public string Information => IsNotFound ? "File not found!" : Duration == null ? "Cannot estimate duration" : $"Duration: {Duration.Value:hh':'mm':'ss}";

    [RelayCommand]
    private void Trigger() => List.AudioManager.Trigger(this);

    [RelayCommand]
    private void Stop() => List.AudioManager.StopAll(this);

    [RelayCommand]
    private void Assign()
    {
        List.Editor.Open(this);
        List.Editor.StartAssigning();
    }

    [RelayCommand]
    public async Task Relink()
    {
        var file = await List.Editor.FilePicker.PickOne(SoundList.Options);
        if (file == null)
            return;
        Path = file;
        UpdateDuration();
        UpdatePlaybackState(SoundState.Stopped);
    }

    [RelayCommand]
    private void Configure() => List.Editor.Open(this);

    public SoundViewModel UpdateDuration()
    {
        Duration = File.Exists(Path) && SoundMetadataReader.Read(Path, ReadOptions) is {IsSuccess: true, Value.Duration: var duration} // TODO: error handling probably
            ? duration
            : null;
        return this;
    }

    public void UpdatePlaybackState(SoundState state) => Dispatcher.UIThread.InvokeOrPost(() =>
    {
        PlaybackState = state;
        if (state != SoundState.Stopped)
            return;
        lock (_suspenders)
        {
            _suspenders.Clear();
        }
    });

    public void Pause(IPlaybackSuspender suspender)
    {
        lock (_suspenders)
        {
            _suspenders.Add(suspender);
        }
    }

    public void Resume(IPlaybackSuspender suspender)
    {
        lock (_suspenders)
        {
            _suspenders.Remove(suspender);
        }
    }

}
