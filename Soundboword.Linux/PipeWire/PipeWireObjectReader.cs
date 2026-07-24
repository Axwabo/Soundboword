namespace Soundboword.Linux.PipeWire;

public static class PipeWireObjectReader
{

    private const string Id = "id ";
    private const string Type = "type ";
    private const string Node = "PipeWire:Interface:Node/3";
    private const string Port = "PipeWire:Interface:Port/3";
    private const string MediaClass = "media.class";
    private const string Name = "node.name";
    private const string Description = "node.description";
    private const string NodeId = "node.id";
    private const string PortId = "port.id";

    public static List<PipeWireObject> ReadObjectsAsync(ReadOnlySpan<char> info)
    {
        var nodes = new List<PipeWireObject>();
        string? id = null, p1 = null, p2 = null, description = null;
        foreach (var line in info.Split('\n'))
        {
            var span = info[line].Trim();
            if (BeginNewEntry(span, ref id, ref p1, ref p2, ref description, nodes) || id == null)
                continue;
            var equals = span.IndexOf('=');
            if (equals == -1)
                continue;
            var propertyName = span[..equals].Trim();
            var value = span[(equals + 1)..].Trim().Trim('"');
            if (propertyName is MediaClass or NodeId)
                p1 = value.ToString();
            else if (propertyName is Name or PortId)
                p2 = value.ToString();
            else if (propertyName is Description)
                description = value.ToString();
        }

        return nodes;
    }

    private static bool BeginNewEntry(ReadOnlySpan<char> line, ref string? id, ref string? p1, ref string? p2, ref string? description, List<PipeWireObject> nodes)
    {
        if (!line.StartsWith(Id))
            return false;
        var comma = line.IndexOf(',');
        var type = line.IndexOf(Type);
        if (comma == -1 || type == -1)
            return false;
        if (id != null && p1 != null && p2 != null)
        {
            if (description != null)
                nodes.Add(new PipeWireNode(id, p1, p2, description));
            else if (int.TryParse(p2, out var portId))
                nodes.Add(new PipeWirePort(id, p1, portId));
        }

        id = line[(type + Type.Length)..].Trim() is Node or Port
            ? line[Id.Length..comma].Trim().ToString()
            : null;
        p1 = null;
        p2 = null;
        description = null;
        return true;
    }

}
