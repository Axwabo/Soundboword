namespace Soundboword.Linux.PipeWire;

[RegisterSingleton<TabListToggles>]
public sealed class LinkTogglesViewModel : TabListToggles
{

    public LinkTogglesViewModel(NodeManager manager) => Manager = manager;

    public NodeManager Manager { get; }

}
