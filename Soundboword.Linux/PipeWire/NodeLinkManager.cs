namespace Soundboword.Linux.PipeWire;

public sealed partial class NodeLinkManager : ObservableObject
{

    public static NodeLinkManager? Create(PipeWireNode source, PipeWireNode destination, List<PipeWirePort> ports)
    {
        var outputs = new List<string>();
        var inputs = new List<string>();
        foreach (var port in ports)
            if (port.Direction == "out" && port.Node == source.Id)
                outputs.Add(port.Id);
            else if (port.Direction == "in" && port.Node == destination.Id)
                inputs.Add(port.Id);
        return outputs.Count != 0 && outputs.Count == inputs.Count
            ? new NodeLinkManager(outputs, inputs)
            : null;
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
    private async Task ToggleLink()
    {
        InProgress = true;
        var links = await PipeWireCli.ListLinksAsync();
        var disconnect = false;
        foreach (var link in links)
        {
            if (!_outputs.Contains(link.OutputPort) || !_inputs.Contains(link.InputPort))
                continue;
            disconnect = true;
            break;
        }

        var success = false;
        for (var i = 0; i < _inputs.Count; i++)
            success |= await PipeWireCli.LinkAsync(_outputs[i], _inputs[i], disconnect);
        if (success)
            IsLinked = !disconnect;
        InProgress = false;
    }

}
