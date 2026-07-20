using System.Diagnostics.CodeAnalysis;
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
        try
        {
            _connection = new DBusConnection(new DBusConnectionOptions(DBusAddress.Session!) {AutoConnect = false});
            _connection.ConnectAsync().AsTask().Wait();
            _sender = _connection.Sender;
            _shortcuts = _connection.CreateShortcuts();
            IsAvailable = true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        /*Console.WriteLine(_sender);
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
        });*/
    }

    public string Name => "XDG Global Shortcuts";

    [MemberNotNullWhen(true, nameof(_connection), nameof(_sender), nameof(_shortcuts))]
    public bool IsAvailable { get; }

    public async Task<IInputMethod?> ActivateAsync()
    {
        await Task.Delay(1000);
        return null;
    }

}
