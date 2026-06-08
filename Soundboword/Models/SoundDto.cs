using System;

namespace Soundboword.Models;

public sealed record SoundDto(Guid Id, string Name, string Path, PlaybackMode Mode, bool Loop);
