using Microsoft.Extensions.Logging;

namespace Soundboword.Logging;

public sealed class LogViewModel : ViewModelBase
{

    public required string Name { get; init; }

    public required LogLevel Level { get; init; }

    public required string Content { get; init; }

}
