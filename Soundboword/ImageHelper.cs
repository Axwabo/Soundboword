using System.Net.Http;
using System.Threading;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Soundboword;

public static class ImageHelper
{

    public static readonly Bitmap NotFound = LoadFromResource(new Uri("avares://Soundboword/Assets/not-found.png"));

    private static readonly HttpClient Client = new(); // TODO: client factory

    public static Bitmap LoadFromResource(Uri resource) => new(AssetLoader.Open(resource));

    public static async Task<Bitmap?> LoadFromWeb(string url, CancellationToken cancellationToken)
    {
        try
        {
#if DEBUG
            await Task.Delay(10000, cancellationToken);
            if (Random.Shared.Next(10) < 2)
                return null;
#endif
            var bytes = await Client.GetByteArrayAsync(url, cancellationToken).ConfigureAwait(false);
            return new Bitmap(new MemoryStream(bytes));
        }
        catch (Exception)
        {
            // TODO: log
            return null;
        }
    }

}
