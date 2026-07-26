using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Microsoft.Extensions.Logging;

namespace Soundboword.Converters;

public sealed class LogLevelConverter : IValueConverter
{

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => (value as LogLevel?) switch
    {
        LogLevel.Error => Brushes.Red,
        LogLevel.Warning => Brushes.Orange,
        _ => null
    };

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();

}
