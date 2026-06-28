using Avalonia.Media;
using Soundboword.Settings;

namespace Soundboword.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
{

    public MainWindowViewModel()
    {
        Board = new BoardViewModel();
        Devices = new DevicesViewModel(new SoundFlowDeviceManager());
        Playbacks = new PlaybacksViewModel();
        Inputs = new InputsViewModel();
        Preferences = new Preferences();
        FilePicker = new FilePicker();
        ShortcutAssigner = new ShortcutAssigner();
    }

    public MainWindowViewModel(BoardViewModel board, DevicesViewModel devices, PlaybacksViewModel playbacks, InputsViewModel inputs, Preferences preferences, FilePicker filePicker, ShortcutAssigner shortcutAssigner)
    {
        Board = board;
        Devices = devices;
        Playbacks = playbacks;
        Inputs = inputs;
        Preferences = preferences;
        FilePicker = filePicker;
        ShortcutAssigner = shortcutAssigner;
    }

    public BoardViewModel Board { get; }

    public DevicesViewModel Devices { get; }

    public PlaybacksViewModel Playbacks { get; }

    public InputsViewModel Inputs { get; }
    public Preferences Preferences { get; }

    public FilePicker FilePicker { get; }

    public ShortcutAssigner ShortcutAssigner { get; }

    [ObservableProperty]
    public partial IBrush PressedBrush { get; set; } = Brushes.Gray;

}
