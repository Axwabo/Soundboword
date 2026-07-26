using Soundboword.Linux.PipeWire.Settings;
using Soundboword.Linux.PipeWire.Wizard;
using Soundboword.Settings;

namespace Soundboword.Linux.PipeWire;

public sealed partial class PipeWireTabViewModel : ViewModelBase
{

    private readonly PipeWireCli _cli;
    private readonly RestartContext? _context;
    private readonly TopLevel? _topLevel;

    public PipeWireTabViewModel()
    {
        _cli = new PipeWireCli();
        NodeManager = new NodeManager(_cli, new SoundFlowDeviceManager());
        SwitchHandler = new LinkRepair(NodeManager, new SettingsManager(new PreferencesProvider(new PipeWirePreferences())));
    }

    public PipeWireTabViewModel(PipeWireCli cli, TopLevel topLevel, RestartContext context, NodeManager nodeManager)
    {
        SwitchHandler = context.SwitchHandler;
        _cli = cli;
        _topLevel = topLevel;
        _context = context;
        NodeManager = nodeManager;
    }

    public DeviceSwitchHandler SwitchHandler { get; }

    public NodeManager NodeManager { get; }

    public Task<bool> IsAvailable => _cli.IsAvailable;

    [RelayCommand]
    private async Task LaunchWizard()
    {
        if (_topLevel is Window parent && _context is not null)
            await PipeWireWizardWindow.ShowDialogAsync(parent, _context);
    }

    [RelayCommand]
    private Task RefreshLinks() => SwitchHandler.OnOutputDeviceSwitched();

}
