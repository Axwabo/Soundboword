using System.Net.Http;
using System.Threading;
using Avalonia.Media.Imaging;

namespace Soundboword;

public static class ImageHelper
{

    private static readonly HttpClient Client = new(); // TODO: client factory

    public static async Task<Bitmap?> LoadFromWeb(string url, CancellationToken cancellationToken)
    {
        try
        {
            var bytes = await Client.GetByteArrayAsync(url, cancellationToken);
            return new Bitmap(new MemoryStream(bytes));
        }
        catch (Exception)
        {
            // TODO: log
            return null;
        }
    }

}
