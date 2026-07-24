using Soundboword.Linux.PipeWire.Wizard;

namespace Soundboword.Linux.PipeWire;

public sealed partial class PipeWireTabViewModel : ViewModelBase
{

    private readonly PipeWireCli? _cli;
    private readonly RestartContext? _context;
    private readonly TopLevel? _topLevel;

    public PipeWireTabViewModel()
    {
    }

    public PipeWireTabViewModel(PipeWireCli cli, TopLevel topLevel, RestartContext context)
    {
        _cli = cli;
        _topLevel = topLevel;
        _context = context;
        _ = ListNodes();
    }

    public Task<bool>? IsAvailable => _cli?.IsAvailable;

    public ObservableCollection<PipeWireNode> Nodes { get; } = [];

    private async Task ListNodes()
    {
        var nodes = await PipeWireCli.ListNodesAsync();
        foreach (var pipeWireNode in nodes)
            Nodes.Add(pipeWireNode);
    }

    [RelayCommand]
    private async Task LaunchWizard()
    {
        if (_topLevel is Window parent && _context is not null)
            await PipeWireWizardWindow.ShowDialogAsync(parent, _context);
    }

}
