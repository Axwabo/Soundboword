using System.Text.Json.Serialization;
using Soundboword.Inputs.Launchpad;

namespace Soundboword;

[JsonSerializable(typeof(IEnumerable<SoundDto>))]
[JsonSerializable(typeof(IEnumerable<string>))]
[JsonSerializable(typeof(Dictionary<string, LaunchpadKey>))]
public sealed partial class SourceGenerationContext : JsonSerializerContext;
