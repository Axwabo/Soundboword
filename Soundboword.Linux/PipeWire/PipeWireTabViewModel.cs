using Soundboword.Linux.PipeWire.Wizard;

namespace Soundboword.Linux.PipeWire;

public sealed partial class PipeWireTabViewModel : ViewModelBase
{

    private readonly AudioManager? _audioManager;

    private readonly PipeWireCli? _cli;
    private readonly DevicesViewModel? _devices;
    private readonly TopLevel? _topLevel;

    public PipeWireTabViewModel()
    {
    }

    public PipeWireTabViewModel(PipeWireCli cli, TopLevel topLevel, AudioManager audioManager, DevicesViewModel devices)
    {
        _cli = cli;
        _topLevel = topLevel;
        _audioManager = audioManager;
        _devices = devices;
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
        if (_topLevel is Window parent && _audioManager != null && _devices != null)
            await PipeWireWizardWindow.ShowDialogAsync(parent, _audioManager, _devices);
    }

}
