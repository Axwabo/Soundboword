using Avalonia.Media;
using Soundboword.Settings;

namespace Soundboword.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
{

    public MainWindowViewModel() : this(new BoardViewModel(),
        new DevicesViewModel(new SoundFlowDeviceManager(new UserData()), new DeviceSwitchHandler()),
        new PlaybacksViewModel(),
        new InputsViewModel(),
        new SettingsManager(),
        new FilePicker(),
        new ShortcutAssigner())
    {
    }

    public MainWindowViewModel(BoardViewModel board, DevicesViewModel devices, PlaybacksViewModel playbacks, InputsViewModel inputs, SettingsManager settingsManager, FilePicker filePicker, ShortcutAssigner shortcutAssigner, ITabsProvider? provider = null, TabListToggles? toggles = null)
    {
        FilePicker = filePicker;
        ShortcutAssigner = shortcutAssigner;
        Toggles = toggles;
        Pages.Add(new TabItemViewModel("Sounds", "🔊", board));
        Pages.Add(new TabItemViewModel("Devices", "🎧", devices));
        Pages.Add(new TabItemViewModel("Playbacks", "🎚️", playbacks));
        Pages.Add(new TabItemViewModel("Inputs", "🎛️", inputs));
        Pages.AddRange(provider?.AdditionalTabs ?? []);
        Pages.Add(new TabItemViewModel("Settings", "⚙️", settingsManager));
    }

    public List<TabItemViewModel> Pages { get; } = [];

    public FilePicker FilePicker { get; }

    public ShortcutAssigner ShortcutAssigner { get; }

    public TabListToggles? Toggles { get; }

    [ObservableProperty]
    public partial IBrush PressedBrush { get; set; } = Brushes.Gray;

}
