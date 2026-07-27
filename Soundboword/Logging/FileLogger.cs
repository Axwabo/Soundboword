namespace Soundboword.Logging;

public sealed class FileLogger : ILogger
{

    private readonly string _name;

    private readonly FileLoggerProvider _provider;

    public FileLogger(string name, FileLoggerProvider provider)
    {
        _name = name;
        _provider = provider;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        => _provider.Enqueue(_name, logLevel, formatter(state, exception), exception);

    public bool IsEnabled(LogLevel logLevel) => logLevel >= LogPreferences.Instance.FileLevel;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

}
