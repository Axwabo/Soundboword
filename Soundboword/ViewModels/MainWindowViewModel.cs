using Soundboword.Logging;
using Soundboword.Settings;

namespace Soundboword.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
{

    private static readonly GridLength ZeroLength = new GridLength(0, GridUnitType.Pixel);

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
        LogsPage = Add(new Tab("Logs", "📒", logList, true));
        UpdateBottomBarStatus();
        CurrentPage = Pages[0];
        LogList.PropertyChanged += HandleBarPropertyChanged;
        LogPreferences.Instance.PropertyChanged += HandleBarPropertyChanged;
    }

    public List<Page> Pages { get; } = [];

    [ObservableProperty]
    public partial Page CurrentPage { get; set; }

    public Page LogsPage { get; }

    public LogListViewModel LogList { get; }

    public FilePicker FilePicker { get; }

    public ShortcutAssigner ShortcutAssigner { get; }

    public TabListToggles? Toggles { get; }

    public GridLength? TogglesRowHeight => Toggles == null ? ZeroLength : GridLength.Star;

    public GridLength? TogglesRowMinHeight => Toggles == null ? ZeroLength : GridLength.Parse("160");

    [ObservableProperty]
    public partial bool ShowBottomBar { get; private set; }

    private void HandleBarPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(LogPreferences.HideBottomBar) or nameof(LogList.Last))
            UpdateBottomBarStatus();
    }

    private void UpdateBottomBarStatus() => ShowBottomBar = !LogPreferences.Instance.HideBottomBar && LogList.Last != null;

    private Page Add(Tab tab)
    {
        var page = new Page
        {
            Tab = tab,
            Parent = this
        };
        Pages.Add(page);
        return page;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName != nameof(CurrentPage))
            return;
        ShortcutAssigner.Close();
        if (CurrentPage.Content is PageModelBase page)
            page.OnActivated();
    }

}
