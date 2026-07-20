using Soundboword.Generated;
using Soundboword.Inputs;
using Tmds.DBus.Protocol;

namespace Soundboword.Linux.Services;

[RegisterSingleton<IInputFactory>(Duplicate = DuplicateStrategy.Append)]
public sealed class GlobalShortcutsFactory : IInputFactory
{

    private readonly DBusConnection? _connection;
    private readonly GlobalShortcuts? _shortcuts;

    public GlobalShortcutsFactory()
    {
        var oldContext = SynchronizationContext.Current;
        try
        {
            SynchronizationContext.SetSynchronizationContext(null);
            _connection = new DBusConnection(new DBusConnectionOptions(DBusAddress.Session!) {AutoConnect = false});
            _connection.ConnectAsync().AsTask().Wait();
            _shortcuts = new GlobalShortcuts(_connection, "org.freedesktop.portal.Desktop", "/org/freedesktop/portal/desktop");
            var sessionHandleToken = $"soundboword_{Guid.CreateVersion7()}\0";
            var session = _shortcuts.CreateSessionAsync(new Dictionary<string, VariantValue>
            {
                {"handle_token", $"/org/freedesktop/portal/desktop/session/Soundboword/{sessionHandleToken}"},
                {"session_handle_token", sessionHandleToken}
            }).Result;
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(oldContext);
        }
    }

    public string Name => "XDG Global Shortcuts";

    public bool IsAvailable => _connection != null;

    public IInputMethod? Activate() => null;

}
