using Avalonia.Input;

namespace Soundboword.Windows.GlobalHotkeys;

public sealed record HotkeyGesture(Key Key, KeyModifiers Modifiers);
