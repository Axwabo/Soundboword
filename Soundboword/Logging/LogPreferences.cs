using System.Text.Json;

namespace Soundboword.Logging;

public sealed partial class LogPreferences : ObservableObject
{

    public static string FilePath { get; } = Path.Combine(UserData.Root, "logging.json");

    public static LogPreferences Instance { get; }

    static LogPreferences()
    {
        Instance = new LogPreferences();
        try
        {
            if (!File.Exists(FilePath))
                return;
            using var file = File.OpenRead(FilePath);
            Instance = JsonSerializer.Deserialize(file, SourceGenerationContext.Default.LogPreferences) ?? new LogPreferences();
        }
        catch
        {
            // ignored
        }
    }

    [ObservableProperty]
    public partial LogLevel FileLevel { get; set; } = LogLevel.Information;

    [ObservableProperty]
    public partial LogLevel AppLevel { get; set; } = LogLevel.Warning;

}
