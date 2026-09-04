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

    public SoundViewModel Sound
    {
        get;
        set
        {
            field = value;
            if (value.Edits != null)
                Edits = value.Edits;
        }
    } = Sample;

    public SoundEdits Edits
    {
        get => field ??= new SoundEdits();
        private set;
    }

    public double Max => Sound.Duration.GetValueOrDefault().TotalSeconds;

    public double Start
    {
        get => Edits.Start.TotalSeconds;
        set => Edits.Start = TimeSpan.FromSeconds(value);
    }

    public double LoopStart
    {
        get => Edits.LoopStart.TotalSeconds;
        set => Edits.LoopStart = TimeSpan.FromSeconds(value);
    }

    public double LoopEnd
    {
        get => Edits.LoopEnd.TotalSeconds;
        set => Edits.LoopEnd = TimeSpan.FromSeconds(value);
    }

}
