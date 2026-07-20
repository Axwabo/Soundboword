using Soundboword.Generated;
using Tmds.DBus.Protocol;
using Response = (uint Response, System.Collections.Generic.Dictionary<string, Tmds.DBus.Protocol.VariantValue> Results);

namespace Soundboword.Linux;

public static class DBusExtensions
{

    extension(DBusConnection connection)
    {

        public static string GenerateToken() => $"soundboword_{Guid.CreateVersion7():N}";

        public string Sender => (connection.UniqueName ?? "").TrimStart(':').Replace('.', '_');

        public async Task<Response> RequestAsync(string sender, Func<Dictionary<string, VariantValue>, Task<ObjectPath>> send)
        {
            // ReSharper disable once InvokeAsExtensionMemberFromSameClass
            var handleToken = GenerateToken();
            ObjectPath expectedPath = $"{DestkopPortal.Path}/{sender}/{handleToken}";
            var request = new Request(connection, DestkopPortal.Bus, expectedPath);
            var tcs = new TaskCompletionSource<Response>();
            using var watcher = await request.WatchResponseAsync(tcs.SetResult, false);
            var objectPath = await send(new Dictionary<string, VariantValue>
            {
                {"handle_token", handleToken}
            }).ConfigureAwait(false);
            return objectPath == expectedPath
                ? await tcs.Task.ConfigureAwait(false)
                : throw new IOException("DBus said nuh uh");
        }

    }

}
