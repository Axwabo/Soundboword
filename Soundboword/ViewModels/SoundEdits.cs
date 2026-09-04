namespace Soundboword.ViewModels;

public sealed partial class SoundEdits : ObservableObject
{

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasStart))]
    public partial TimeSpan? Start { get; set; }

    [ObservableProperty]
    public partial TimeSpan? End { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasLoopStart))]
    public partial TimeSpan? LoopStart { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasLoopEnd))]
    public partial TimeSpan? LoopEnd { get; set; }

    public bool HasStart
    {
        get => Start.HasValue;
        set => Start = value ? Start ?? TimeSpan.Zero : null;
    }

    public bool HasLoopStart
    {
        get => LoopStart.HasValue;
        set => LoopStart = value ? LoopStart ?? TimeSpan.Zero : null;
    }

    public bool HasLoopEnd
    {
        get => LoopEnd.HasValue;
        set => LoopEnd = value ? LoopEnd ?? TimeSpan.Zero : null;
    }

}
