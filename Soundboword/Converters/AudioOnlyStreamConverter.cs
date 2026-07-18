using System.Globalization;
using Avalonia.Data.Converters;
using YoutubeExplode.Videos.Streams;

namespace Soundboword.Converters;

public sealed class AudioOnlyStreamConverter : IValueConverter
{

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value switch
    {
        AudioOnlyStreamInfo {AudioLanguage: { } language} info => $"{info.Bitrate} {info.AudioCodec}, {language}",
        AudioOnlyStreamInfo info => $"{info.Bitrate} {info.AudioCodec}",
        _ => null
    };

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();

}
