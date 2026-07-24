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
    private const string Direction = "port.direction";
    private const string PortId = "port.id";

    public static List<PipeWireObject> ReadObjectsAsync(ReadOnlySpan<char> info)
    {
        var nodes = new List<PipeWireObject>();
        string? id = null, type = null, p1 = null, p2 = null, p3 = null;
        foreach (var line in info.Split('\n'))
        {
            var span = info[line].Trim();
            if (BeginNewEntry(span, ref id, ref type, ref p1, ref p2, ref p3, nodes) || id == null)
                continue;
            var equals = span.IndexOf('=');
            if (equals == -1)
                continue;
            var propertyName = span[..equals].Trim();
            var value = span[(equals + 1)..].Trim().Trim('"');
            if (propertyName is MediaClass or NodeId)
                p1 = value.ToString();
            else if (propertyName is Name or Direction)
                p2 = value.ToString();
            else if (propertyName is Description or PortId)
                p3 = value.ToString();
        }

        return nodes;
    }

    private static bool BeginNewEntry(ReadOnlySpan<char> line, ref string? id, ref string? type, ref string? p1, ref string? p2, ref string? p3, List<PipeWireObject> nodes)
    {
        if (!line.StartsWith(Id))
            return false;
        var comma = line.IndexOf(',');
        var typeIndex = line.IndexOf(Type);
        if (comma == -1 || typeIndex == -1)
            return false;
        if (id != null && p1 != null && p2 != null && p3 != null)
        {
            if (type is Node)
                nodes.Add(new PipeWireNode(id, p1, p2, p3));
            else if (int.TryParse(p3, out var portId))
                nodes.Add(new PipeWirePort(id, p1, p2, portId));
        }

        var typeSpan = line[(typeIndex + Type.Length)..].Trim();
        (id, type) = typeSpan is Node or Port
            ? (line[Id.Length..comma].Trim().ToString(), typeSpan.ToString())
            : (null, null);
        p1 = null;
        p2 = null;
        p3 = null;
        return true;
    }

    public static HashSet<PipeWireLink> ReadLinksAsync(ReadOnlySpan<char> info)
    {
        foreach (var range in info.Split('\n'))
        {
        }
    }

}
