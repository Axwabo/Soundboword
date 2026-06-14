using System.Threading;
using Avalonia.Media.Imaging;
using YoutubeExplode.Videos;

namespace Soundboword.YouTube;

public sealed class YouTubeVideo : IDisposable
{

    private readonly CancellationTokenSource _cts = new();

    public YouTubeVideo(IVideo video)
    {
        Id = video.Id;
        Title = video.Title;
        Author = video.Author.ChannelTitle;
        Duration = video.Duration;
        var thumbnail = video.Thumbnails.OrderByDescending(e => e.Resolution.Height).FirstOrDefault();
        Thumbnail = thumbnail == null ? Task.FromResult<Bitmap?>(null) : ImageHelper.LoadFromWeb(thumbnail.Url, _cts.Token);
    }

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
