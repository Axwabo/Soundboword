using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace Soundboword.Converters;

public sealed class LogLevelConverter : IValueConverter
{

    private static readonly IImmutableSolidColorBrush Info = new ImmutableSolidColorBrush(Color.FromArgb(255, 0, 0, 100));
    private static readonly IImmutableSolidColorBrush Error = new ImmutableSolidColorBrush(Color.FromArgb(255, 150, 0, 0));
    private static readonly IImmutableSolidColorBrush Warning = new ImmutableSolidColorBrush(Color.FromArgb(255, 150, 120, 0));

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => (value as LogLevel?) switch
    {
        LogLevel.Debug => Brushes.Black,
        LogLevel.Information => Info,
        LogLevel.Error => Error,
        LogLevel.Warning => Warning,
        _ => null
    };

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();

}
