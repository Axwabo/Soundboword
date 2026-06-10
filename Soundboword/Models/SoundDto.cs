using System;

namespace Soundboword.Models;

public sealed record SoundDto(Guid Id, string Name, string Path, TriggerMode Mode, bool Loop, float Volume = 1);
