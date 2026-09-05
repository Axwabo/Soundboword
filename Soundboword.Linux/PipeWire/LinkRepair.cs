using Soundboword.Linux.PipeWire.Settings;
using Soundboword.Settings;

namespace Soundboword.Linux.PipeWire;

[RegisterSingleton<DeviceSwitchHandler>(Duplicate = DuplicateStrategy.Replace)]
public sealed class LinkRepair : DeviceSwitchHandler
{

    private readonly NodeManager _nodeManager;
    private readonly PipeWirePreferences _preferences;

    private bool _everRefreshed;

    private PipeWireNode? _previousMic;

    public LinkRepair(NodeManager nodeManager, SettingsManager settingsManager)
    {
        _nodeManager = nodeManager;
        _preferences = settingsManager.Require<PipeWirePreferences>();
    }

    protected override async Task OnOutputDeviceSwitchedAsync()
    {
        await Task.Delay(200);
        var output = _nodeManager.OutputNode;
        var hearMyself = _nodeManager.HearMyself;
        await _nodeManager.RefreshAsync(
            _nodeManager.HearSounds?.IsLinked,
            _everRefreshed
                ? _nodeManager.MicSounds?.IsLinked
                : _preferences.AutoMicSounds,
            _everRefreshed
                ? _nodeManager.MicPassthrough?.IsLinked
                : _preferences.AutoPassthrough,
            _nodeManager.HearMyself?.IsLinked
        );
        if (output != _nodeManager.OutputNode && hearMyself is {IsLinked: true})
            await hearMyself.ToggleLink(false, _nodeManager.Links);
        _everRefreshed = true;
    }

    protected override async Task OnMicrophoneSwitchedAsync()
    {
        if (_previousMic == _nodeManager.PhysicalMicrophone)
            return;
        _previousMic = _nodeManager.PhysicalMicrophone;
        var disconnectPassthrough = _nodeManager.MicPassthrough?.ToggleLink(false, _nodeManager.Links) ?? Task.CompletedTask;
        var disconnectHearMyself = _nodeManager.HearMyself?.ToggleLink(false, _nodeManager.Links) ?? Task.CompletedTask;
        var (passthrough, hearMyself) = _nodeManager.UpdatePhysicalMic(_nodeManager.MicPassthrough?.IsLinked, _nodeManager.HearMyself?.IsLinked);
        await Task.WhenAll(disconnectPassthrough, disconnectHearMyself, passthrough, hearMyself);
    }

}
