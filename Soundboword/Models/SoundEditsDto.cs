using System.Text.Json.Serialization;

namespace Soundboword.Models;

public sealed record SoundEditsDto(
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    TimeSpan? Start,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    TimeSpan? End,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    TimeSpan? LoopStart,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    TimeSpan? LoopEnd
)
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
