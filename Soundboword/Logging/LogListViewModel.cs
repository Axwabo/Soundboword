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
        Last = log;
    }

}
