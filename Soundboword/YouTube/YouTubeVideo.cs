using System.Threading;
using Avalonia.Media.Imaging;
using YoutubeExplode.Common;
using YoutubeExplode.Videos;

namespace Soundboword.YouTube;

public sealed class YouTubeVideo : IVideo, IDisposable
{

    private const double OptimalResolution = 16 / 9d;

    private static readonly Author DefaultAuthor = new(default, "Author");

    private static readonly TaskCompletionSource<Bitmap?> Tcs = new();

    private readonly CancellationTokenSource _cts = new();

    private readonly IVideo? _source;

    private YouTubeVideo(IVideo video)
    {
        _source = video;
        var thumbnail = video.Thumbnails
            .OrderBy(e => Math.Abs(OptimalResolution - e.Resolution.Width / (double) e.Resolution.Height))
            .ThenByDescending(e => e.Resolution.Height)
            .FirstOrDefault();
        Thumbnail = thumbnail == null ? Task.FromResult<Bitmap?>(null) : ImageHelper.LoadFromWeb(thumbnail.Url, _cts.Token);
    }

    public YouTubeVideo(IVideo? source, Task<Bitmap?> thumbnail)
    {
        _source = source;
        Thumbnail = thumbnail;
    }

    private YouTubeVideo() => Thumbnail = Tcs.Task;

    public static YouTubeVideo Loading { get; } = new();

    public string Author => ((IVideo) this).Author.ChannelTitle;

    public Task<Bitmap?> Thumbnail { get; }

    public DateTimeOffset? UploadDate => (_source as Video)?.UploadDate;

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
        if (Thumbnail.IsCompletedSuccessfully)
            Thumbnail.Result?.Dispose();
    }

    public VideoId Id => _source?.Id ?? default;

    string IVideo.Url => _source?.Url ?? "";

    public string Title => _source?.Title ?? "";

    Author IVideo.Author => _source?.Author ?? DefaultAuthor;

    public TimeSpan? Duration => _source?.Duration;

    IReadOnlyList<Thumbnail> IVideo.Thumbnails => _source?.Thumbnails ?? ReadOnlyCollection<Thumbnail>.Empty;

    public static YouTubeVideo Create(IVideo video) => video as YouTubeVideo ?? new YouTubeVideo(video);

}
