namespace Soundboword.Models;

public sealed record Shortcut(string InputMethodName, string FriendlyName, ShortcutAction Action);
