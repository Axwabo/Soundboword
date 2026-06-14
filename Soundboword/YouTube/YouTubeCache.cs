using System.Threading;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos;
using Container = YoutubeExplode.Videos.Streams.Container;

namespace Soundboword.YouTube;

public static class YouTubeCache
{

    private static readonly string Folder = Path.Combine(UserData.Folder, "YouTube");

    private static void EnsureDirectory() => Directory.CreateDirectory(Folder);

    public static async Task<string> CacheAsync(VideoId videoId, IProgress<double> progress, CancellationToken cancellationToken)
    {
        EnsureDirectory();
        using var client = new YoutubeClient();
        var manifest = await client.Videos.Streams.GetManifestAsync(videoId, cancellationToken).ConfigureAwait(false);
        var streamInfo = manifest.GetAudioOnlyStreams().OrderByDescending(e => e.Bitrate).First();
        var path = Path.Combine(Folder, $"{videoId}.mp3");
        var request = new ConversionRequestBuilder(path)
            .SetPreset(ConversionPreset.Medium)
            .SetContainer(Container.Mp3)
            .Build();
        await client.Videos.DownloadAsync([streamInfo], request, progress, cancellationToken);
        return path;
    }

}
