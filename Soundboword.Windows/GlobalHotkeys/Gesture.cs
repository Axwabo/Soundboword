using System.Text.Json.Serialization;
using Avalonia.Input;

namespace Soundboword.Windows.GlobalHotkeys;

public sealed record Gesture(
    [property: JsonConverter(typeof(JsonStringEnumConverter<Key>))]
    Key Key,
    [property: JsonConverter(typeof(JsonStringEnumConverter<KeyModifiers>))]
    KeyModifiers Modifiers
);
