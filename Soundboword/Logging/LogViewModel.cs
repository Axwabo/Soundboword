namespace Soundboword.Logging;

public sealed class LogViewModel : ViewModelBase
{

    public required string Name { get; init; }

    public required LogLevel Level { get; init; }

    public required string Content { get; init; }

    public Exception? Exception { get; init; }

    public string Formatted
    {
        get => field ??= Exception == null ? $"[{Name}] {Content}" : $"[{Name}] {Content}\n{Exception.Message}";
        init;
    }

    public int MaxLines { get; set; }

}
