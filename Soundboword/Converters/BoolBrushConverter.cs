using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Soundboword.Converters;

public sealed class BoolBrushConverter : IValueConverter
{

    public IBrush? True { get; set; }

    public IBrush? False { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? True : False;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();

}
