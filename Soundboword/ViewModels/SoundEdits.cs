namespace Soundboword.ViewModels;

public sealed partial class SoundEdits : ObservableObject
{

    [ObservableProperty]
    public partial TimeSpan Start { get; set; }

    [ObservableProperty]
    public partial TimeSpan End { get; set; }

    [ObservableProperty]
    public partial TimeSpan LoopStart { get; set; }

    [ObservableProperty]
    public partial TimeSpan LoopEnd { get; set; }

    [ObservableProperty]
    public partial bool HasStart { get; set; }

    [ObservableProperty]
    public partial bool HasLoopStart { get; set; }

    [ObservableProperty]
    public partial bool HasLoopEnd { get; set; }

}
