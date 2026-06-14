using System.Threading;
using Avalonia.Media.Imaging;
using YoutubeExplode.Videos;

namespace Soundboword.YouTube;

public sealed class YouTubeVideo : IDisposable
{

    private const double OptimalResolution = 16 / 9d;

    private static readonly TaskCompletionSource<Bitmap?> Tcs = new();

    private readonly CancellationTokenSource _cts = new();

    public YouTubeVideo(IVideo video)
    {
        Id = video.Id;
        Title = video.Title;
        Author = video.Author.ChannelTitle;
        Duration = video.Duration;
        var thumbnail = video.Thumbnails
            .OrderBy(e => Math.Abs(OptimalResolution - e.Resolution.Width / (double) e.Resolution.Height))
            .ThenByDescending(e => e.Resolution.Height)
            .FirstOrDefault();
        Thumbnail = thumbnail == null ? Task.FromResult<Bitmap?>(null) : ImageHelper.LoadFromWeb(thumbnail.Url, _cts.Token);
    }

    private YouTubeVideo()
    {
        Id = default;
        Title = "Title";
        Author = "Author";
        Duration = TimeSpan.Zero;
        Thumbnail = Tcs.Task;
    }

    public static YouTubeVideo Loading { get; } = new();

    public VideoId Id { get; }

    public string Title { get; }

    public string Author { get; }

    public TimeSpan? Duration { get; }

    public Task<Bitmap?> Thumbnail { get; }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
        if (Thumbnail.IsCompletedSuccessfully)
            Thumbnail.Result?.Dispose();
    }

}
