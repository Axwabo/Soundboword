using Soundboword.OutputDevices;

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

}
