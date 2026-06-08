using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Soundboword.Inputs.Launchpad;

namespace Soundboword.Converters;

public sealed class LaunchpadKeyValueConverter : IValueConverter
{

    public static LaunchpadKeyValueConverter Instance { get; } = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => ((LaunchpadKey?) value ?? default(LaunchpadKey)) switch
    {
        LaunchpadKey.Top1 => "🔝1",
        LaunchpadKey.Top2 => "🔝2",
        LaunchpadKey.Top3 => "🔝3",
        LaunchpadKey.Top4 => "🔝4",
        LaunchpadKey.Top5 => "🔝5",
        LaunchpadKey.Top6 => "🔝6",
        LaunchpadKey.Top7 => "🔝7",
        LaunchpadKey.Top8 => "🔝8",
        var key => key.ToString()
    };

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();

}
