using SoundFlow.Components;
using SoundFlow.Providers;

namespace Soundboword.Models;

public sealed record Sound(StreamDataProvider Provider, SoundPlayer Player);
