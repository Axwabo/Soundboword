using System.Collections.Concurrent;

namespace Soundboword.Logging;

[ProviderAlias("Avalonia")]
public sealed class AvaloniaLoggerProvider : ILoggerProvider
{

    private readonly LogListViewModel _list;

    private readonly ConcurrentDictionary<string, AvaloniaLogger> _logers = new(StringComparer.OrdinalIgnoreCase);

    public AvaloniaLoggerProvider(LogListViewModel list) => _list = list;

    public ILogger CreateLogger(string categoryName)
        => _logers.GetOrAdd(categoryName, name => new AvaloniaLogger(name, _list));

    public void Dispose() => _logers.Clear();

}
