using Soundboword.Logging;
using Soundboword.Settings;

namespace Soundboword.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
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
        Add(new Tab("Sounds", "🔊", board));
        Add(new Tab("Devices", "🎧", devices));
        Add(new Tab("Playbacks", "🎚️", playbacks));
        Add(new Tab("Inputs", "🎛️", inputs));
        foreach (var tab in provider?.AdditionalTabs ?? [])
            Add(tab);
        Add(new Tab("Settings", "⚙️", settingsManager));
        Add(new Tab("Logs", "📒", logList, true));
        UpdateBottomBarStatus();
        LogList.PropertyChanged += HandlePropertyChanged;
        LogPreferences.Instance.PropertyChanged += HandlePropertyChanged;
    }

    public List<Page> Pages { get; } = [];

    [ObservableProperty]
    public partial Page CurrentPage { get; set; }

    public LogListViewModel LogList { get; }

    public FilePicker FilePicker { get; }

    public ShortcutAssigner ShortcutAssigner { get; }

    public TabListToggles? Toggles { get; }

    [ObservableProperty]
    public partial bool ShowBottomBar { get; private set; }

    private void HandlePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(LogPreferences.HideBottomBar) or nameof(LogList.Last))
            UpdateBottomBarStatus();
    }

    private void UpdateBottomBarStatus() => ShowBottomBar = !LogPreferences.Instance.HideBottomBar && LogList.Last != null;

    private void Add(Tab tab) => Pages.Add(new Page
    {
        Tab = tab,
        Parent = this
    });

}
