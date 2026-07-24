namespace Soundboword.Linux.PipeWire;

public sealed class PipeWireTabViewModel : ViewModelBase
{

    private readonly PipeWireCli? _cli;

    public PipeWireTabViewModel()
    {
    }

    public PipeWireTabViewModel(PipeWireCli cli) => _cli = cli;

    public Task<bool>? IsAvailable => _cli?.IsAvailable;

}
