using Soundboword.Generated;
using Soundboword.Inputs;
using Tmds.DBus.Protocol;

namespace Soundboword.Linux.Services;

[RegisterSingleton<IInputFactory>(Duplicate = DuplicateStrategy.Append)]
public sealed class GlobalShortcutsFactory : IInputFactory
{

    private readonly DBusConnection? _connection;
    private readonly string? _sender;
    private readonly GlobalShortcuts? _shortcuts;

    public GlobalShortcutsFactory()
    {
        _connection = new DBusConnection(new DBusConnectionOptions(DBusAddress.Session!) {AutoConnect = false});
        _connection.ConnectAsync().GetAwaiter().GetResult();
        _sender = _connection.Sender;
        Console.WriteLine(_sender);
        _shortcuts = _connection.CreateShortcuts();
        _connection.RequestAsync(_sender, values => _shortcuts.CreateSessionAsync(values.WithSessionHandleToken())).ContinueWith(task =>
        {
            try
            {
                var (response, results) = task.Result;
                Console.WriteLine(response);
                Console.WriteLine(results);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        });
    }

    public string Name => "XDG Global Shortcuts";

    public bool IsAvailable => _connection != null;

    public IInputMethod? Activate() => null;

}
