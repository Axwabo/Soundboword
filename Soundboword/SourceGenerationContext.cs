using System.Text.Json.Serialization;
using Soundboword.Models;

namespace Soundboword;

[JsonSourceGenerationOptions]
[JsonSerializable(typeof(SoundDto))]
internal partial class SourceGenerationContext : JsonSerializerContext;
