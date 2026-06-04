namespace Soundboword.Models;

public sealed record SoundDto(string Name, string Path, PlaybackMode Mode, bool Loop);
