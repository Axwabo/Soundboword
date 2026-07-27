using static Microsoft.Extensions.Logging.LogLevel;

namespace Soundboword.Logging;

public static class LogLevelExtensions
{

    public static IReadOnlyList<LogLevel> Levels { get; } =
    [
        Trace,
        Debug,
        Information,
        Warning,
        Error,
        Critical,
        None
    ];

    extension(LogLevel level)
    {

        public string ToStringFast() => level switch
        {
            Trace => nameof(Trace),
            Debug => nameof(Debug),
            Information => nameof(Information),
            Warning => nameof(Warning),
            Error => nameof(Error),
            Critical => nameof(Critical),
            None => nameof(None),
            _ => ""
        };

    }

}
