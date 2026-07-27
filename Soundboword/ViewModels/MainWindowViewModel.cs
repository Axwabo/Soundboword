using Soundboword.Logging;
using Soundboword.Settings;

namespace Soundboword.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{

    public MainWindowViewModel() : this(new BoardViewModel(),
        new DevicesViewModel(new SoundFlowDeviceManager(), new DeviceSwitchHandler()),
        new PlaybacksViewModel(),
        new InputsViewModel(),
        new SettingsManager(),
        new LogListViewModel(),
        new FilePicker(),
        new ShortcutAssigner())
    {
    }

    public MainWindowViewModel(BoardViewModel board, DevicesViewModel devices, PlaybacksViewModel playbacks, InputsViewModel inputs, SettingsManager settingsManager, LogListViewModel logList, FilePicker filePicker, ShortcutAssigner shortcutAssigner, ITabsProvider? provider = null, TabListToggles? toggles = null)
    {
        LogList = logList;
        FilePicker = filePicker;
        ShortcutAssigner = shortcutAssigner;
        Toggles = toggles;
        Pages.Add(new TabItemViewModel("Sounds", "🔊", board));
        Pages.Add(new TabItemViewModel("Devices", "🎧", devices));
        Pages.Add(new TabItemViewModel("Playbacks", "🎚️", playbacks));
        Pages.Add(new TabItemViewModel("Inputs", "🎛️", inputs));
        Pages.AddRange(provider?.AdditionalTabs ?? []);
        Pages.Add(new TabItemViewModel("Settings", "⚙️", settingsManager));
        Pages.Add(new TabItemViewModel("Logs", "📒", logList));
        LogList.PropertyChanged += HandlePropertyChanged;
        LogPreferences.Instance.PropertyChanged += HandlePropertyChanged;
    }

    public List<TabItemViewModel> Pages { get; } = [];

    public LogListViewModel LogList { get; }

    public FilePicker FilePicker { get; }

    public ShortcutAssigner ShortcutAssigner { get; }

    public TabListToggles? Toggles { get; }

    public bool ShowBottomBar { get; private set; }

    private void HandlePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(LogPreferences.HideBottomBar) or nameof(LogList.Last))
            UpdateBottomBarStatus();
    }

    private void UpdateBottomBarStatus()
    {
        ShowBottomBar = !LogPreferences.Instance.HideBottomBar && LogList.Last != null;
        OnPropertyChanged(nameof(ShowBottomBar));
    }

}
