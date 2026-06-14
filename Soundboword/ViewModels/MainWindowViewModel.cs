using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Soundboword.Services;

namespace Soundboword.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
{

    public MainWindowViewModel()
    {
        Board = new BoardViewModel();
        Devices = new DevicesViewModel(new SoundFlowDeviceManager());
        Playbacks = new PlaybacksViewModel();
        Inputs = new InputsViewModel();
        FilePicker = new FilePicker();
        ShortcutAssigner = new ShortcutAssigner();
    }

    public MainWindowViewModel(BoardViewModel board, DevicesViewModel devices, PlaybacksViewModel playbacks, InputsViewModel inputs, FilePicker filePicker, ShortcutAssigner shortcutAssigner)
    {
        Board = board;
        Devices = devices;
        Playbacks = playbacks;
        Inputs = inputs;
        FilePicker = filePicker;
        ShortcutAssigner = shortcutAssigner;
    }

    public BoardViewModel Board { get; }

    public DevicesViewModel Devices { get; }

    public PlaybacksViewModel Playbacks { get; }

    public InputsViewModel Inputs { get; }

    public FilePicker FilePicker { get; }

    public ShortcutAssigner ShortcutAssigner { get; }

    [ObservableProperty]
    public partial IBrush PressedBrush { get; set; } = Brushes.Gray;

}
