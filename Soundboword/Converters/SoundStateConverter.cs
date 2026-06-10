using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Soundboword.Converters;

public sealed class SoundStateConverter : IMultiValueConverter
{

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture) => values[(int) (parameter ?? 0)];

}
