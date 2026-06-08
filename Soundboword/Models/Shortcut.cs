using Soundboword.ViewModels;

namespace Soundboword.Models;

public readonly record struct Shortcut(string MethodName, SoundViewModel Sound, string FriendlyName);
