using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Microsoft.Extensions.Logging.Abstractions;

namespace Soundboword;

[RegisterSingleton]
public sealed partial class Lifetime
{

    private readonly List<(ShutdownPriority Priority, Action Action)> _callbacks = [];

    private ILogger _logger = NullLogger.Instance;

    public Lifetime(IClassicDesktopStyleApplicationLifetime? lifetime = null)
    {
        IsActive = lifetime != null;
        lifetime?.Exit += (_, _) => ShutdownServices();
        if (IsActive)
            Dispatcher.UIThread.UnhandledException += (_, args) =>
            {
                LogException(args.Exception);
                ShutdownServices();
            };
    }

    public bool IsActive { get; }

    public void InitializeLogging(ILoggerProvider provider) => _logger = provider.CreateLogger("Lifetime");

    public event Action Exit
    {
        add => Register(value);
        remove => throw new NotSupportedException();
    }

    private void ShutdownServices()
    {
        _callbacks.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        foreach (var (_, action) in _callbacks)
            action();
    }

    public void Register(Action callback, ShutdownPriority priority = ShutdownPriority.Normal)
        => _callbacks.Add((priority, callback));

    [LoggerMessage(LogLevel.Critical, "Exception in the UI thread")]
    private partial void LogException(Exception exception);

}

public enum ShutdownPriority
{

    Normal = 0,
    Final = int.MaxValue

}
