using Soundboword.Linux.PipeWire.Wizard;

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
    }

    public PipeWireTabViewModel(PipeWireCli cli, TopLevel topLevel, RestartContext context)
    {
        _cli = cli;
        _topLevel = topLevel;
        _context = context;
        NodeManager = context.NodeManager;
    }

    public NodeManager NodeManager { get; }

    public Task<bool> IsAvailable => _cli.IsAvailable;

    [RelayCommand]
    private async Task LaunchWizard()
    {
        if (_topLevel is Window parent && _context is not null)
            await PipeWireWizardWindow.ShowDialogAsync(parent, _context);
    }

}
