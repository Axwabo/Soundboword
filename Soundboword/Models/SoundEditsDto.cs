namespace Soundboword.Models;

public sealed record SoundEditsDto(TimeSpan? Start, TimeSpan? End, TimeSpan? LoopStart, TimeSpan? LoopEnd)
{

    public static implicit operator SoundEditsDto?(SoundEdits? edits) => edits == null
        ? null
        : new SoundEditsDto(edits.Start, edits.End, edits.LoopStart, edits.LoopEnd);

    public SoundEdits ToModel() => new()
    {
        Start = Start,
        End = End,
        LoopStart = LoopStart,
        LoopEnd = LoopEnd
    };

}
