using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Soundboword.Models;

namespace Soundboword.Converters;

public sealed class PlaybackModeConverter : IValueConverter
{

    private const string StartStopConst = "Start -> Stop";
    private const string StartRestartConst = "Start -> Restart";
    private const string DuplicateConst = "Duplicate";

    public static string StartStop => StartStopConst;
    public static string StartRestart => StartRestartConst;
    public static string Duplicate => DuplicateConst;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => ((PlaybackMode?) value ?? default(PlaybackMode)) switch
    {
        PlaybackMode.StartRestart => StartRestartConst,
        PlaybackMode.Duplicate => DuplicateConst,
        _ => StartStopConst
    };

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => (value as string) switch
    {
        StartRestartConst => PlaybackMode.StartRestart,
        DuplicateConst => PlaybackMode.Duplicate,
        _ => PlaybackMode.StartStop
    };

}
