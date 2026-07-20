using Soundboword.Generated;
using Tmds.DBus.Protocol;

namespace Soundboword.Linux;

internal delegate Task<ObjectPath> SendPortalRequest(GlobalShortcuts shortcuts, Dictionary<string, VariantValue> options);

public static class DBusExtensions
{

    private const string Bus = "org.freedesktop.portal.Desktop";
    private const string Path = "/org/freedesktop/portal/desktop";

    extension(Dictionary<string, VariantValue> dictionary)
    {

        public Dictionary<string, VariantValue> WithSessionHandleToken()
        {
            dictionary["session_handle_token"] = DBusConnection.GenerateToken();
            return dictionary;
        }

    }

    extension(DBusConnection connection)
    {

        public static string GenerateToken() => $"soundboword_{Guid.CreateVersion7():N}";

        public string Sender => (connection.UniqueName ?? "").TrimStart(':').Replace('.', '_');

        internal GlobalShortcuts CreateShortcuts() => new(connection, Bus, Path);

        internal async Task<PortalResponse> RequestAsync(string sender, GlobalShortcuts shortcuts, SendPortalRequest send, CancellationToken cancellationToken = default)
        {
            // ReSharper disable once InvokeAsExtensionMemberFromSameClass
            var handleToken = GenerateToken();
            ObjectPath expectedPath = $"{Path}/request/{sender}/{handleToken}";
            var request = new Request(connection, Bus, expectedPath);
            var tcs = new TaskCompletionSource<PortalResponse>();
            cancellationToken.Register(() =>
            {
                request.CloseAsync();
                tcs.SetResult((1, []));
            });
            using var watcher = await request.WatchResponseAsync(result => tcs.SetResult(cancellationToken.IsCancellationRequested ? (1, []) : result), false);
            if (cancellationToken.IsCancellationRequested)
                return (1, []);
            var objectPath = await send(shortcuts, new Dictionary<string, VariantValue>
            {
                {"handle_token", handleToken}
            }).ConfigureAwait(false);
            return objectPath == expectedPath
                ? await tcs.Task.ConfigureAwait(false)
                : throw new IOException("DBus said nuh uh");
        }

    }

}
