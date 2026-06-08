using System.Text.Json.Serialization;

namespace Soundboword.Models;

public abstract record Shortcut(string InputMethodName, string FriendlyName);
