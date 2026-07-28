using System.Text.Json;
using Soundboword.Settings;

namespace Soundboword.Logging;

public sealed partial class LogPreferences : SettingsSection
{

    private static readonly string FilePath = Path.Combine(UserData.Root, "logging.json");

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

    [ObservableProperty]
    public partial bool HideBottomBar { get; set; }

    public override void Save()
    {
        try
        {
            using var file = File.Create(FilePath);
            JsonSerializer.Serialize(file, this, SourceGenerationContext.Default.LogPreferences);
        }
        catch
        {
            // ignored
        }
    }

}
