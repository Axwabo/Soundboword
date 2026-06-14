namespace Soundboword.Models;

public readonly record struct SoundId(Guid Value)
{

    public string String { get; } = Value.ToString();

    public static implicit operator SoundId(Guid guid) => new(guid);

    public static implicit operator Guid(SoundId id) => id.Value;

    public static implicit operator string(SoundId id) => id.String;

}
