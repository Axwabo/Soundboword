using SoundFlow.Components;
using SoundFlow.Providers;

namespace Soundboword.Models;

public sealed record SoundPlayback(StreamDataProvider Provider, SoundPlayer Player);
