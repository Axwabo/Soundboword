using System.Collections.Generic;
using System.Text.Json.Serialization;
using Soundboword.Models;

namespace Soundboword;

[JsonSourceGenerationOptions]
[JsonSerializable(typeof(IEnumerable<SoundDto>))]
internal partial class SourceGenerationContext : JsonSerializerContext;
