using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Soundboword.Converters;

public sealed class StringEqualityConverter : IValueConverter
{

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is string s && parameter is string p && s == p;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();

}
