using System.Diagnostics.CodeAnalysis;
using Soundboword.Generated;
using Tmds.DBus.Protocol;

namespace Soundboword.Linux.Services;

[RegisterSingleton]
public sealed class GlobalShortcutsPortal
{

    private readonly DBusConnection? _connection;
    private readonly string? _sender;
    private readonly GlobalShortcuts? _shortcuts;

    private readonly TopLevel _topLevel;

    public GlobalShortcutsPortal(TopLevel topLevel)
    {
        _topLevel = topLevel;
        try
        {
            _connection = new DBusConnection(new DBusConnectionOptions(DBusAddress.Session!) {AutoConnect = false});
            _connection.ConnectAsync().AsTask().Wait();
            _sender = _connection.Sender;
            _shortcuts = _connection.CreateShortcuts();
            IsAvailable = true;
            SessionHandle = CreateSessionAsync();
        }
        catch (Exception e)
        {
            SessionHandle = Task.FromException<ObjectPath>(e);
            Console.WriteLine(e);
        }
    }

    public Task<ObjectPath> SessionHandle { get; }

    public string ParentWindow => _topLevel.TryGetPlatformHandle() switch
    {
        {Handle: var handle, HandleDescriptor: "X11"} => $"x11:{handle}",
        // TODO: Wayland when Avalonia officially supports it
        _ => ""
    };

    [MemberNotNullWhen(true, nameof(_connection), nameof(_sender), nameof(_shortcuts))]
    public bool IsAvailable { get; }

    internal Task<PortalResponse> RequestAsync(SendPortalRequest send, CancellationToken cancellationToken = default)
        => IsAvailable
            ? _connection.RequestAsync(_sender, _shortcuts, send, cancellationToken)
            : throw new InvalidOperationException("Portal unavailable");

    internal async ValueTask<IDisposable> WatchActivatedAsync(Action<string> callback)
        => IsAvailable
            ? await _shortcuts.WatchActivatedAsync(tuple =>
            {
                if (SessionHandle.IsCompletedSuccessfully && SessionHandle.Result == tuple.SessionHandle)
                    callback(tuple.ShortcutId);
            })
            : throw new InvalidOperationException("Portal unavailable");

    private async Task<ObjectPath> CreateSessionAsync()
    {
        var (response, results) = await RequestAsync((shortcuts, options) => shortcuts.CreateSessionAsync(options.WithSessionHandleToken())).ConfigureAwait(false);
        return response == 0
            ? results["session_handle"].GetString()
            : throw new IOException($"Code {response}");
    }

}
