using System.Threading;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using Container = YoutubeExplode.Videos.Streams.Container;

namespace Soundboword.YouTube;

// TODO: to service
public static class YouTubeCache
{

    private static readonly string Folder = Path.Combine(UserData.Folder, "YouTube");

    public static Container Wav { get; } = new("wav");

    private static void EnsureDirectory() => Directory.CreateDirectory(Folder);

    public static async Task<string> CacheAsync(VideoId id, IStreamInfo? info, IProgress<double> progress, Container container, CancellationToken cancellationToken)
    {
        EnsureDirectory();
        using var client = new YoutubeClient();
        if (info == null)
        {
            var manifest = await client.Videos.Streams.GetManifestAsync(id, cancellationToken).ConfigureAwait(false);
            info = manifest.GetAudioOnlyStreams()
                .OrderByDescending(e => e.Bitrate)
                .First();
        }

        var path = Path.Combine(Folder, $"{id}.{container.Name}");
        var request = new ConversionRequestBuilder(path)
            .SetPreset(ConversionPreset.Medium)
            .SetContainer(container)
            .Build();
        await client.Videos.DownloadAsync([info], request, progress, cancellationToken).ConfigureAwait(false);
        return path;
    }

}
