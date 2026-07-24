using Soundboword.Linux.PipeWire.Wizard;

namespace Soundboword.Linux.PipeWire;

public sealed partial class PipeWireTabViewModel : ViewModelBase
{

    private readonly PipeWireCli? _cli;
    private readonly TopLevel? _topLevel;

    public PipeWireTabViewModel()
    {
    }

    public PipeWireTabViewModel(PipeWireCli cli, TopLevel topLevel)
    {
        _cli = cli;
        _topLevel = topLevel;
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
        if (_topLevel is Window parent)
            await PipeWireWizardWindow.ShowDialogAsync(parent);
    }

}
