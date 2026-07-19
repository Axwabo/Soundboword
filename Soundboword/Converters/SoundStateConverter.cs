using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace Soundboword.Converters;

public sealed class SoundStateConverter : IValueConverter
{

    private static readonly ImmutableSolidColorBrush Playing = new(Color.Parse("#1100ff00"));
    private static readonly ImmutableSolidColorBrush Paused = new(Color.Parse("#11ffff00"));
    private static readonly ImmutableSolidColorBrush Error = new(Color.Parse("#22ff0000"));

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => ((SoundState?) value ?? default(SoundState)) switch
    {
        SoundState.Playing => Playing,
        SoundState.Paused => Paused,
        SoundState.NotFound => Error,
        _ => null
    };

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();

}
