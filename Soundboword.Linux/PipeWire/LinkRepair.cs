using Soundboword.OutputDevices;

namespace Soundboword.Linux.PipeWire;

[RegisterSingleton<DeviceSwitchHandler>(Duplicate = DuplicateStrategy.Replace)]
public sealed class LinkRepair : DeviceSwitchHandler
{

    private readonly NodeManager _nodeManager;

    private Task? _relinkTask;

    public LinkRepair(NodeManager nodeManager) => _nodeManager = nodeManager;

    public override Task OnDeviceSwitchedAsync()
    {
        if (_relinkTask is not {IsCompleted: false})
            _relinkTask = Relink();
        return _relinkTask;
    }

    private async Task Relink()
    {
        IsSwitching = true;
        await Task.Delay(200);
        var output = _nodeManager.OutputNode;
        var hearMyself = _nodeManager.HearMyself;
        await _nodeManager.RefreshAsync(_nodeManager.HearSounds?.IsLinked, _nodeManager.MicSounds?.IsLinked, _nodeManager.MicPassthrough?.IsLinked, _nodeManager.HearMyself?.IsLinked);
        if (output != _nodeManager.OutputNode && hearMyself is {IsLinked: true})
            await hearMyself.ToggleLink(false, _nodeManager.Links);
        IsSwitching = false;
    }

}
