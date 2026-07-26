using System.Diagnostics.CodeAnalysis;
using Soundboword.Generated;
using Tmds.DBus.Protocol;

namespace Soundboword.Linux.Shortcuts;

[RegisterSingleton]
public sealed partial class GlobalShortcutsPortal
{

    private readonly DBusConnection? _connection;
    private readonly ILogger _logger;
    private readonly string? _sender;
    private readonly GlobalShortcuts? _shortcuts;

    private readonly TopLevel _topLevel;

    public GlobalShortcutsPortal(TopLevel topLevel, ILoggerFactory loggerFactory)
    {
        _topLevel = topLevel;
        _logger = loggerFactory.CreateLogger("XDG Global Shortcuts Portal");
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
            LogConnectionFailure(e);
            SessionHandle = Task.FromException<ObjectPath>(e);
        }
    }

    public Task<ObjectPath> SessionHandle { get; }

    public string ParentWindow => _topLevel.TryGetPlatformHandle() switch
    {
        {Handle: var handle, HandleDescriptor: "XID"} => $"x11:{handle}",
        // TODO: Wayland when Avalonia officially supports it
        _ => ""
    };

    [MemberNotNullWhen(true, nameof(_connection), nameof(_sender), nameof(_shortcuts))]
    public bool IsAvailable { get; }

    internal async Task<PortalResponse> RequestAsync(SendPortalRequest send, CancellationToken cancellationToken = default)
        => IsAvailable
            ? await _connection.RequestAsync(_sender, _shortcuts, send, cancellationToken)
            : (2, []);

    internal async ValueTask<IDisposable?> WatchActivatedAsync(Action<string> callback)
        => IsAvailable
            ? await _shortcuts.WatchActivatedAsync(tuple =>
            {
                if (SessionHandle.IsCompletedSuccessfully && SessionHandle.Result == tuple.SessionHandle)
                    callback(tuple.ShortcutId);
            })
            : null;

    private async Task<ObjectPath> CreateSessionAsync()
    {
        var (response, results) = await RequestAsync((shortcuts, options) => shortcuts.CreateSessionAsync(options.WithSessionHandleToken())).ConfigureAwait(false);
        return response == 0
            ? results["session_handle"].GetString()
            : throw new IOException($"Response code {response}");
    }

    [LoggerMessage(LogLevel.Error, "Failed to create D-Bus session")]
    private partial void LogConnectionFailure(Exception exception);

}
