namespace Soundboword.Editor;

[RegisterScoped]
public sealed class EditorContext
{

    private static readonly SoundViewModel Sample = new()
    {
        Id = Guid.CreateVersion7(),
        List = null!,
        Name = "amogus",
        Path = "/sus/amogus.wav",
        Duration = TimeSpan.FromSeconds(5)
    };

    public SoundViewModel Sound { get; set; } = Sample;

    public SoundEdits Edits { get; } = new();

    public double Max => Sound.Duration.GetValueOrDefault().TotalSeconds;

    public double Start
    {
        get => Edits.Start.GetValueOrDefault().TotalSeconds;
        set => Edits.Start = TimeSpan.FromSeconds(value);
    }

    public double LoopStart
    {
        get => Edits.LoopStart.GetValueOrDefault().TotalSeconds;
        set => Edits.LoopStart = TimeSpan.FromSeconds(value);
    }

    public double LoopEnd
    {
        get => Edits.LoopEnd.GetValueOrDefault().TotalSeconds;
        set => Edits.LoopEnd = TimeSpan.FromSeconds(value);
    }

}
