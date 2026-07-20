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

    public GlobalShortcutsPortal()
    {
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

    [MemberNotNullWhen(true, nameof(_connection), nameof(_sender), nameof(_shortcuts))]
    public bool IsAvailable { get; }

    internal Task<PortalResponse> RequestAsync(SendPortalRequest send)
        => IsAvailable
            ? _connection.RequestAsync(_sender, _shortcuts, send)
            : throw new InvalidOperationException("Portal unavailable");

    private async Task<ObjectPath> CreateSessionAsync()
    {
        var (response, results) = await RequestAsync((shortcuts, options) => shortcuts.CreateSessionAsync(options.WithSessionHandleToken())).ConfigureAwait(false);
        return response == 0
            ? results["session_handle"].GetString()
            : throw new IOException($"Code {response}");
    }

}
