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
        _connection = new DBusConnection(new DBusConnectionOptions(DBusAddress.Session!) {AutoConnect = false});
        _connection.ConnectAsync().AsTask().Wait();
        _shortcuts = DestkopPortal.CreateShortcuts(_connection);
    }

    public string Name => "XDG Global Shortcuts";

    public bool IsAvailable => _connection != null;

    public IInputMethod? Activate() => null;

}
