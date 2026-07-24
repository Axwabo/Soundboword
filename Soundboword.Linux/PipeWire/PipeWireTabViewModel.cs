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
    }

    public Task<bool>? IsAvailable => _cli?.IsAvailable;

    [RelayCommand]
    private async Task LaunchWizard()
    {
        if (_topLevel is not Window parent)
            return;
        var wizard = new PipeWireWizardWindow();
        await wizard.ShowDialog(parent);
    }

}
