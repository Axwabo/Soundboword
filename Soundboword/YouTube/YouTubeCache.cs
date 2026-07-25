using System.Threading;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using Container = YoutubeExplode.Videos.Streams.Container;

namespace Soundboword.YouTube;

[RegisterScoped]
public sealed class YouTubeCache
{

    private static readonly string Folder = Path.Combine(UserData.Root, "YouTube");

    private static void EnsureDirectory() => Directory.CreateDirectory(Folder);

    public static Container Wav { get; } = new("wav");

    private readonly YoutubeClient _client;

    public YouTubeCache(YoutubeClient client) => _client = client;

    public async Task<string> CacheAsync(VideoId id, IStreamInfo? info, IProgress<double> progress, Container container, CancellationToken cancellationToken)
    {
        EnsureDirectory();
        if (info == null)
        {
            var manifest = await _client.Videos.Streams.GetManifestAsync(id, cancellationToken).ConfigureAwait(false);
            info = manifest.GetAudioOnlyStreams()
                .OrderByDescending(e => e.Bitrate)
                .First();
        }

        var path = Path.Combine(Folder, $"{id}.{container.Name}");
        var request = new ConversionRequestBuilder(path)
            .SetPreset(ConversionPreset.Medium)
            .SetContainer(container)
            .Build();
        await _client.Videos.DownloadAsync([info], request, progress, cancellationToken).ConfigureAwait(false);
        return path;
    }

}
