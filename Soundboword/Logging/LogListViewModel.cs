namespace Soundboword.Logging;

[RegisterSingleton(Registration = RegistrationStrategy.Self)]
public sealed partial class LogListViewModel : ViewModelBase
{

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

}
