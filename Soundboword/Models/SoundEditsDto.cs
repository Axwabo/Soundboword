using System.Text.Json.Serialization;

namespace Soundboword.Models;

public sealed record SoundEditsDto(
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    TimeSpan? Start,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    TimeSpan? End, // TODO: provider does not support it
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    TimeSpan? LoopStart,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    TimeSpan? LoopEnd
)
{

    public static implicit operator SoundEditsDto?(SoundEdits? edits) => edits == null
        ? null
        : new SoundEditsDto(
            edits.HasStart ? edits.Start : null,
            null,
            edits.HasLoopStart ? edits.LoopStart : null,
            edits.HasLoopEnd ? edits.LoopEnd : null
        );

    public SoundEdits ToModel() => new()
    {
        Start = Start.GetValueOrDefault(),
        LoopStart = LoopStart.GetValueOrDefault(),
        LoopEnd = LoopEnd.GetValueOrDefault()
    };

}
