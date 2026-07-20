using Soundboword.Generated;
using Tmds.DBus.Protocol;
using Response = (uint Response, System.Collections.Generic.Dictionary<string, Tmds.DBus.Protocol.VariantValue> Results);

namespace Soundboword.Linux;

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

        public async Task<Response> RequestAsync(string sender, Func<Dictionary<string, VariantValue>, Task<ObjectPath>> send)
        {
            // ReSharper disable once InvokeAsExtensionMemberFromSameClass
            var handleToken = GenerateToken();
            ObjectPath expectedPath = $"{Path}/request/{sender}/{handleToken}";
            var request = new Request(connection, Bus, expectedPath);
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
