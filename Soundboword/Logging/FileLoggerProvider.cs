using System.Collections.Concurrent;
using System.Threading;

namespace Soundboword.Logging;

[ProviderAlias("File")]
[RegisterSingleton<ILoggerProvider>(Duplicate = DuplicateStrategy.Append)]
public sealed class FileLoggerProvider : ILoggerProvider
{

    private static readonly string Folder = Path.Combine(UserData.Root, "Logs");
    private static readonly Memory<char> TimeBuffer = new char[20].AsMemory();

    private readonly CancellationTokenSource _cts = new();

    private readonly ConcurrentDictionary<string, FileLogger> _loggers = new(StringComparer.OrdinalIgnoreCase);

    private readonly ConcurrentQueue<(DateTimeOffset Time, string Name, LogLevel Level, string Content, Exception? Exception)> _logs = [];
    private readonly TimeSpan _offset;

    private readonly StreamWriter _writer;

    public FileLoggerProvider(Lifetime lifetime)
    {
        Directory.CreateDirectory(Folder);
        var now = DateTimeOffset.Now;
        _offset = now.Offset;
        _writer = new StreamWriter(File.Create(Path.Combine(Folder, now.ToString("yyyy-MM-dd_HH-mm-ss'.txt'"))));
        lifetime.Register(StopLoggingAndFlush, ShutdownPriority.Final);
        lifetime.InitializeLogging(this);
        Task.Run(WriteAsync);
    }

    public ILogger CreateLogger(string categoryName)
        => _loggers.GetOrAdd(categoryName, name => new FileLogger(name, this));

    public void Dispose() => _loggers.Clear();

    private async Task WriteAsync()
    {
        var token = _cts.Token;
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
        try
        {
            DateTimeOffset.Now.ToOffset(_offset).TryFormat(TimeBuffer.Span, out var offsetCharsWritten, "zzz");
            await _writer.WriteAsync("Logging started, timezone offset: ");
            await _writer.WriteLineAsync(TimeBuffer[..offsetCharsWritten], CancellationToken.None);
            while (await timer.WaitForNextTickAsync(token))
            {
                while (!token.IsCancellationRequested && _logs.TryDequeue(out var tuple))
                {
                    tuple.Time.ToOffset(_offset).TryFormat(TimeBuffer.Span, out var charsWritten, "s");
                    await _writer.WriteAsync(TimeBuffer[..charsWritten], CancellationToken.None);
                    await _writer.WriteAsync(" [");
                    await _writer.WriteAsync(tuple.Level.ToStringFast());
                    await _writer.WriteAsync("] [");
                    await _writer.WriteAsync(tuple.Name);
                    await _writer.WriteAsync("] ");
                    await _writer.WriteLineAsync(tuple.Content);
                    if (tuple.Exception != null)
                        await _writer.WriteLineAsync(tuple.Exception.ToString());
                }

                await _writer.FlushAsync(token);
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            Enqueue("File Logger", LogLevel.Error, "Error while writing logs", e);
        }
    }

    private void StopLoggingAndFlush()
    {
        _cts.Cancel();
        try
        {
            while (_logs.TryDequeue(out var tuple))
            {
                tuple.Time.TryFormat(TimeBuffer.Span, out var charsWritten, "s");
                _writer.Write(TimeBuffer[..charsWritten]);
                _writer.Write(" [");
                _writer.Write(tuple.Level.ToStringFast());
                _writer.Write("] [");
                _writer.Write(tuple.Name);
                _writer.Write("] ");
                _writer.WriteLine(tuple.Content);
                if (tuple.Exception != null)
                    _writer.WriteLine(tuple.Exception.ToString());
            }
        }
        finally
        {
            _cts.Dispose();
            _writer.Dispose();
        }
    }

    public void Enqueue(string name, LogLevel logLevel, string content, Exception? exception)
        => _logs.Enqueue((DateTimeOffset.Now, name, logLevel, content, exception));

}
