using Avalonia.Threading;
using Soundboword.Linux.PipeWire.Wizard;

namespace Soundboword.Linux.PipeWire;

public sealed partial class PipeWireTabViewModel : ViewModelBase
{

    private static readonly Comparison<PipeWirePort> PortComparison = (a, b) => a.PortId.CompareTo(b.PortId);

    private readonly PipeWireCli _cli;
    private readonly RestartContext? _context;
    private readonly TopLevel? _topLevel;

    public PipeWireTabViewModel()
    {
        _cli = new PipeWireCli();
        NodeManager = new NodeManager(_cli);
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

    [ObservableProperty]
    public partial bool PhysicalMicrophonePassthrough { get; private set; }

    [ObservableProperty]
    public partial bool TogglingPassthrough { get; private set; }

    [RelayCommand]
    private async Task LaunchWizard()
    {
        if (_topLevel is Window parent && _context is not null)
            await PipeWireWizardWindow.ShowDialogAsync(parent, _context);
    }

    [RelayCommand]
    private async Task ToggleMicrophonePassthrough()
    {
        TogglingPassthrough = true;
        var disconnect = PhysicalMicrophonePassthrough;
        var passthrough = false;
        try
        {
            if (NodeManager.PhysicalMicrophone is not {Id: var physicalId} || NodeManager.MicNode is not {Id: var micId})
                return;
            var physicalPorts = NodeManager.Ports.Where(e => e.Direction == "out" && e.Node == physicalId).ToList();
            var virtualPorts = NodeManager.Ports.Where(e => e.Direction == "in" && e.Node == micId).ToList();
            if (physicalPorts.Count != virtualPorts.Count)
                return;
            physicalPorts.Sort(PortComparison);
            virtualPorts.Sort(PortComparison);
            var tasks = new Task<bool>[physicalPorts.Count];
            for (var i = 0; i < tasks.Length; i++)
                tasks[i] = PipeWireCli.LinkAsync(physicalPorts[i].Id, virtualPorts[i].Id, disconnect);
            var results = await Task.WhenAll(tasks);
            passthrough = results.All(e => e != disconnect);
        }
        finally
        {
            TogglingPassthrough = false;
            Dispatcher.UIThread.InvokeOrPost(() => PhysicalMicrophonePassthrough = passthrough);
        }
    }

}
