namespace Soundboword.Linux.PipeWire;

[RegisterSingleton<TabListToggles>]
public sealed class LinkTogglesViewModel : TabListToggles
{

    public LinkTogglesViewModel(NodeManager manager, DevicesViewModel devices)
    {
        Manager = manager;
        Devices = devices;
    }

    public NodeManager Manager { get; }

    public DevicesViewModel Devices { get; }

}
