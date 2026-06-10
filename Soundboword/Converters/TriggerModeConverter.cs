using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Soundboword.Models;

namespace Soundboword.Converters;

public sealed class TriggerModeConverter : IValueConverter
{

    private const string StartStopConst = "Start -> Stop";
    private const string StartRestartConst = "Start -> Restart";
    private const string PlayPauseConst = "Play -> Pause";
    private const string DuplicateConst = "Duplicate";

    public static IReadOnlyList<TriggerMode> PlaybackModes { get; } = [TriggerMode.StartStop, TriggerMode.StartRestart, TriggerMode.PlayPause, TriggerMode.Duplicate];

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => ((TriggerMode?) value ?? default(TriggerMode)) switch
    {
        TriggerMode.StartRestart => StartRestartConst,
        TriggerMode.PlayPause => PlayPauseConst,
        TriggerMode.Duplicate => DuplicateConst,
        _ => StartStopConst
    };

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => (value as string) switch
    {
        StartRestartConst => TriggerMode.StartRestart,
        DuplicateConst => TriggerMode.Duplicate,
        PlayPauseConst => TriggerMode.PlayPause,
        _ => TriggerMode.StartStop
    };

}
