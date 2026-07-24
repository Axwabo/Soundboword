namespace Soundboword.Linux.PipeWire;

public sealed class PipeWireTabViewModel : ViewModelBase
{

    private readonly PipeWireCli? _cli;

    public PipeWireTabViewModel() => IsAvailable = Task.FromResult<string>("Preview");

    public PipeWireTabViewModel(PipeWireCli cli)
    {
        _cli = cli;
        IsAvailable = Sus(cli);
    }

    public Task<string> IsAvailable { get; }

    private async Task<string> Sus(PipeWireCli cli)
    {
        var available = await cli.IsAvailable;
        return available.ToString();
    }

}
