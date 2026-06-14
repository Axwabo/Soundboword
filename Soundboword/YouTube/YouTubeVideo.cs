using System.Threading;
using Avalonia.Media.Imaging;
using YoutubeExplode.Common;
using YoutubeExplode.Videos;

namespace Soundboword.YouTube;

public sealed class YouTubeVideo : IDisposable
{

    private static readonly Resolution DefaultResolution = new(1280, 720);

    private readonly CancellationTokenSource _cts = new();

    public YouTubeVideo(IVideo video)
    {
        Id = video.Id;
        Title = video.Title;
        Author = video.Author.ChannelTitle;
        Duration = video.Duration;
        var thumbnail = video.Thumbnails.OrderByDescending(e => e.Resolution.Height).FirstOrDefault();
        Thumbnail = thumbnail == null ? Task.FromResult<Bitmap?>(null) : ImageHelper.LoadFromWeb(thumbnail.Url, _cts.Token);
        ThumbnailResolution = thumbnail?.Resolution ?? DefaultResolution;
    }

    public VideoId Id { get; }

    public string Title { get; }

    public string Author { get; }

    public TimeSpan? Duration { get; }

    public Task<Bitmap?> Thumbnail { get; }

    public Resolution ThumbnailResolution { get; }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
        if (Thumbnail.IsCompletedSuccessfully)
            Thumbnail.Result?.Dispose();
    }

}
