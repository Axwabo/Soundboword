namespace Soundboword.Linux.PipeWire;

public sealed partial class NodeLinkManager : ObservableObject
{

    public static (NodeLinkManager?, Task) Create(PipeWireNode? source, PipeWireNode? destination, List<PipeWirePort> ports, List<PipeWireLink> links, bool? targetState)
    {
        if (source is null || destination is null)
            return (null, Task.CompletedTask);
        var outputs = new List<string>();
        var inputs = new List<string>();
        foreach (var port in ports)
            if (port.Direction == "out" && port.Node == source.Id)
                outputs.Add(port.Id);
            else if (port.Direction == "in" && port.Node == destination.Id)
                inputs.Add(port.Id);
        if (outputs.Count == 0 || outputs.Count != inputs.Count)
            return (null, Task.CompletedTask);
        if (targetState == null)
            return (new NodeLinkManager(outputs, inputs) {IsLinked = IsConnected(links, outputs, inputs)}, Task.CompletedTask);
        var manager = new NodeLinkManager(outputs, inputs) {IsLinked = targetState.Value};
        return (manager, manager.ToggleLink(targetState, links));
    }

    private static bool IsConnected(List<PipeWireLink> links, List<string> outputs, List<string> inputs)
    {
        foreach (var link in links)
            if (outputs.Contains(link.OutputPort) && inputs.Contains(link.InputPort))
                return true;
        return false;
    }

    private readonly List<string> _inputs;

    private readonly List<string> _outputs;

    private NodeLinkManager(List<string> outputs, List<string> inputs)
    {
        _outputs = outputs;
        _inputs = inputs;
    }

    [ObservableProperty]
    public partial bool IsLinked { get; private set; }

    [ObservableProperty]
    public partial bool InProgress { get; private set; }

    [RelayCommand]
    private async Task ToggleLink() => await ToggleLink(null, null);

    public async Task ToggleLink(bool? connect, List<PipeWireLink>? links)
    {
        InProgress = true;
        try
        {
            var disconnect = !connect ?? IsConnected(links ?? await PipeWireCli.ListLinksAsync(), _outputs, _inputs);
            var success = false;
            for (var i = 0; i < _inputs.Count; i++)
                success |= await PipeWireCli.LinkAsync(_outputs[i], _inputs[i], disconnect);
            if (success)
                IsLinked = !disconnect;
            await Task.Delay(100);
        }
        finally
        {
            InProgress = false;
        }
    }

}
