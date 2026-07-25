using SoundFlow.Components;
using SoundFlow.Providers;

namespace Soundboword.Playbacks;

public sealed record SoundPlayback(StreamDataProvider Provider, SoundPlayer Player, string Name);
