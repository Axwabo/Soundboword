using Soundboword.Models;

namespace Soundboword.Inputs.Launchpad;

public sealed record LaunchpadShortcut(string InputMethodName, LaunchpadKey Key) : Shortcut(InputMethodName, Key.FriendlyName);

