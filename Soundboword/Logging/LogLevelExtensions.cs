namespace Soundboword.Logging;

public static class LogLevelExtensions
{

    extension(LogLevel level)
    {

        public string ToStringFast() => level switch
        {
            LogLevel.Trace => nameof(LogLevel.Trace),
            LogLevel.Debug => nameof(LogLevel.Debug),
            LogLevel.Information => nameof(LogLevel.Information),
            LogLevel.Warning => nameof(LogLevel.Warning),
            LogLevel.Error => nameof(LogLevel.Error),
            LogLevel.Critical => nameof(LogLevel.Critical),
            LogLevel.None => nameof(LogLevel.None),
            _ => ""
        };

    }

}
