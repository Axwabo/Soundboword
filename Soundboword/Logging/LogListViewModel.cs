using System.Text.Json;

namespace Soundboword.Logging;

[RegisterSingleton(Registration = RegistrationStrategy.Self)]
public sealed partial class LogListViewModel : ViewModelBase
{

    private static void SaveSettings()
    {
        try
        {
            using var file = File.Create(LogPreferences.FilePath);
            JsonSerializer.Serialize(file, LogPreferences.Instance, SourceGenerationContext.Default.LogPreferences);
        }
        catch
        {
            // ignored
        }
    }

    public LogListViewModel()
    {
    }

    public LogListViewModel(Lifetime lifetime) => lifetime.Register(SaveSettings, ShutdownPriority.Final);

    public ObservableCollection<LogViewModel> Logs { get; } = [];

    [ObservableProperty]
    public partial LogViewModel? Last { get; private set; }

    public void Add(LogViewModel log)
    {
        Logs.Add(log);
        Last = new LogViewModel
        {
            Name = log.Name,
            Level = log.Level,
            Content = log.Content,
            Formatted = log.Formatted,
            MaxLines = 1
        };
    }

    [RelayCommand]
    private void Clear()
    {
        Logs.Clear();
        Last = null;
    }

    [RelayCommand]
    private void ClearLast() => Last = null;

}
