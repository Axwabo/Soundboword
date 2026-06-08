using Soundboword.ViewModels;

namespace Soundboword.Models;

public sealed record Shortcut(string InputMethodName, string FriendlyName, SoundViewModel Sound);
