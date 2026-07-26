using Microsoft.Extensions.Logging;

namespace Soundboword.Logging;

public sealed class LogViewModel : ViewModelBase
{

    public LogViewModel(string name, LogLevel logLevel, string content)
    {
        Name = name;
        LogLevel = logLevel;
        Content = content;
    }

    public string Name { get; }

    public LogLevel LogLevel { get; }

    public string Content { get; }

}
