using Avalonia.Threading;
using Microsoft.Extensions.Logging;

namespace Soundboword.Logging;

public sealed class AvaloniaLogger : ILogger
{

    private readonly LogListViewModel _list;

    private readonly string _name;

    public AvaloniaLogger(string name, LogListViewModel list)
    {
        _name = name;
        _list = list;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;
        var content = formatter(state, exception);
        var log = new LogViewModel
        {
            Name = _name,
            Level = logLevel,
            Content = content
        };
        Dispatcher.UIThread.InvokeOrPost(() => _list.Add(log));
    }

    public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Warning; // TODO: config

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

}
