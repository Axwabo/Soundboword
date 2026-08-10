using System.Text.Json.Serialization;
using Soundboword.Windows.GlobalHotkeys;

namespace Soundboword.Windows;

[JsonSerializable(typeof(HotkeyGesture))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext;
