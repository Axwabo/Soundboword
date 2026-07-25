using System.Text.Json.Serialization;
using Soundboword.Linux.PipeWire.Settings;

namespace Soundboword.Linux;

[JsonSerializable(typeof(SettingsDto))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext;
