using System.Text.Json.Serialization;
using Soundboword.Windows.GlobalHotkeys;

namespace Soundboword.Windows;

[JsonSerializable(typeof(Dictionary<string, Gesture>))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext;
