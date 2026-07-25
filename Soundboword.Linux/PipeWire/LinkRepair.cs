namespace Soundboword.Linux.PipeWire;

[RegisterSingleton<DeviceSwitchHandler>(Duplicate = DuplicateStrategy.Replace)]
public sealed class LinkRepair : DeviceSwitchHandler
{

    private readonly NodeManager _nodeManager;

    private PipeWireNode? _previousMic;

    public LinkRepair(NodeManager nodeManager) => _nodeManager = nodeManager;

    public override async Task OnOutputDeviceSwitchedAsync()
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

    public override async Task OnMicrophoneSwitchedAsync()
    {
        if (IsSwitching || _previousMic == _nodeManager.PhysicalMicrophone)
            return;
        IsSwitching = true;
        _previousMic = _nodeManager.PhysicalMicrophone;
        var disconnectPassthrough = _nodeManager.MicPassthrough?.ToggleLink(false, _nodeManager.Links) ?? Task.CompletedTask;
        var disconnectHearMyself = _nodeManager.HearMyself?.ToggleLink(false, _nodeManager.Links) ?? Task.CompletedTask;
        var (passthrough, hearMyself) = _nodeManager.UpdatePhysicalMic(_nodeManager.MicPassthrough?.IsLinked, _nodeManager.HearMyself?.IsLinked);
        await Task.WhenAll(disconnectPassthrough, disconnectHearMyself, passthrough, hearMyself);
        IsSwitching = false;
    }

}
