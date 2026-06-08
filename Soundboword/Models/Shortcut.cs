using Avalonia.Data.Converters;
using Soundboword.ViewModels;

namespace Soundboword.Models;

public readonly record struct Shortcut<T>(string MethodName, SoundViewModel Sound, T Value, IValueConverter Converter) : IShortcut
{

    object? IShortcut.Value => Value;

}
