namespace Soundboword.Linux.PipeWire;

public sealed partial class NodeLinkManager : ObservableObject
{

    public static NodeLinkManager? Create(PipeWireNode source, PipeWireNode destination, List<PipeWirePort> ports)
    {
        var sources = new List<PipeWirePort>();
        var destinations = new List<PipeWirePort>();
        foreach (var port in ports)
            if (port.Direction == "out" && port.Node == source.Id)
                sources.Add(port);
            else if (port.Direction == "in" && port.Node == destination.Id)
                destinations.Add(port);
        return sources.Count != 0 && sources.Count == destinations.Count
            ? new NodeLinkManager(source, destination, sources, destinations)
            : null;
    }

    private readonly PipeWireNode _destination;
    private readonly List<PipeWirePort> _destinations;

    private readonly PipeWireNode _source;
    private readonly List<PipeWirePort> _sources;

    private NodeLinkManager(PipeWireNode source, PipeWireNode destination, List<PipeWirePort> sources, List<PipeWirePort> destinations)
    {
        _source = source;
        _destination = destination;
        _sources = sources;
        _destinations = destinations;
    }

    [ObservableProperty]
    public partial bool IsLinked { get; private set; }

    [ObservableProperty]
    public partial bool InProgress { get; private set; }

    [RelayCommand]
    private async Task ToggleLink()
    {
        var links = await PipeWireCli.ListLinksAsync();
    }

}
