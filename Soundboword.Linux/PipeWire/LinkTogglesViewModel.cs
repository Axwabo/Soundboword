namespace Soundboword.Linux.PipeWire;

[RegisterSingleton<TabListToggles>]
public sealed class LinkTogglesViewModel : TabListToggles
{

    public LinkTogglesViewModel(NodeManager manager, DeviceSwitchHandler switchHandler)
    {
        Manager = manager;
        SwitchHandler = switchHandler;
    }

    public NodeManager Manager { get; }

    public DeviceSwitchHandler SwitchHandler { get; }

    public override IAsyncRelayCommand? GetCommand(string id)
    {
        if (SwitchHandler.IsSwitching)
            return null;
        var manager = id switch
        {
            LinkToggleAction.HearSounds => Manager.HearSounds,
            LinkToggleAction.MicSounds => Manager.MicSounds,
            LinkToggleAction.MicPassthrough => Manager.MicPassthrough,
            LinkToggleAction.HearMyself => Manager.HearMyself,
            _ => null
        };
        return manager?.ToggleLinkCommand;
    }

}
