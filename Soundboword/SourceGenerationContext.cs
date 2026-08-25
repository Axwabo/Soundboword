using System.Text.Json.Serialization;
using Soundboword.Inputs.Launchpad;
using Soundboword.Logging;
using Soundboword.Settings.General;

namespace Soundboword;

[JsonSerializable(typeof(IEnumerable<SoundDto>))]
[JsonSerializable(typeof(IEnumerable<string>))]
[JsonSerializable(typeof(Dictionary<string, LaunchpadKey>))]
[JsonSerializable(typeof(Preferences))]
[JsonSerializable(typeof(LogPreferences))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext;
