using Avalonia.Media;
using Soundboword.Settings;

namespace Soundboword.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
{

    public MainWindowViewModel() : this(new BoardViewModel(),
        new DevicesViewModel(new SoundFlowDeviceManager()),
        new PlaybacksViewModel(),
        new InputsViewModel(),
        new Preferences(),
        new FilePicker(),
        new ShortcutAssigner())
    {
    }

    public MainWindowViewModel(BoardViewModel board, DevicesViewModel devices, PlaybacksViewModel playbacks, InputsViewModel inputs, Preferences preferences, FilePicker filePicker, ShortcutAssigner shortcutAssigner)
    {
        FilePicker = filePicker;
        ShortcutAssigner = shortcutAssigner;
        Pages.Add(new TabItemViewModel("Sounds", "🔊", board));
        Pages.Add(new TabItemViewModel("Devices", "🎧", devices));
        Pages.Add(new TabItemViewModel("Playbacks", "🎚️", playbacks));
        Pages.Add(new TabItemViewModel("Inputs", "🎛️", inputs));
        Pages.Add(new TabItemViewModel("Settings", "⚙️", preferences));
    }

    public List<TabItemViewModel> Pages { get; } = [];

    public FilePicker FilePicker { get; }

    public ShortcutAssigner ShortcutAssigner { get; }

    [ObservableProperty]
    public partial IBrush PressedBrush { get; set; } = Brushes.Gray;

}
