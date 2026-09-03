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

}
