using Avalonia.Data.Converters;
using Soundboword.ViewModels;

namespace Soundboword.Models;

public interface IShortcut
{

    string MethodName { get; }

    SoundViewModel Sound { get; }

    object? Value { get; }

    IValueConverter Converter { get; }

}
