namespace Soundboword.Logging;

public sealed class LogViewModel : ViewModelBase
{

    public required string Name { get; init; }

    public required LogLevel Level { get; init; }

    public required string Content { get; init; }

    public string Formatted
    {
        get => field ??= $"[{Name}] {Content}";
        init;
    }

    public int MaxLines { get; set; }

}
