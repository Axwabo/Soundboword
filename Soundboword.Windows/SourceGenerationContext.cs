using System.Text.Json.Serialization;
using Avalonia.Input;

namespace Soundboword.Windows;

[JsonSerializable(typeof(Dictionary<string, KeyGesture>))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext;
