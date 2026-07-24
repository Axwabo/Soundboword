namespace Soundboword.Linux.PipeWire;

public static class PipeWireNodeReader
{

    private const string Id = "id ";
    private const string Type = "type ";
    private const string InterfaceNode = "PipeWire:Interface:Node/3";
    private const string MediaClass = "media.class";
    private const string Description = "node.description";

    public static List<PipeWireNode> ReadAudioNodesAsync(StringReader reader)
    {
        var nodes = new List<PipeWireNode>();
        string? id = null, @class = null, description = null;
        string? line;
        while (!string.IsNullOrWhiteSpace(line = reader.ReadLine()))
        {
            var span = line.AsSpan().Trim();
            if (BeginNewDevice(span, ref id, ref @class, ref description, nodes))
                continue;
            var equals = span.IndexOf('=');
            if (equals == -1)
                continue;
            DetectProperty(span, equals, MediaClass, ref @class);
            DetectProperty(span, equals, Description, ref description);
        }

        return nodes;
    }

    private static void DetectProperty(ReadOnlySpan<char> line, int equals, string propertyName, ref string? value)
    {
        if (line[..equals].Trim() == propertyName)
            value = line[(equals + 1)..].Trim().Trim('"').ToString();
    }

    private static bool BeginNewDevice(ReadOnlySpan<char> line, ref string? id, ref string? @class, ref string? description, List<PipeWireNode> nodes)
    {
        if (!line.StartsWith(Id))
            return false;
        var comma = line.IndexOf(',');
        var type = line.IndexOf(Type);
        if (comma == -1 || type == -1 || line[(type + Type.Length)..].Trim() != InterfaceNode)
            return false;
        if (id != null && @class != null && description != null && @class.StartsWith("Audio/"))
            nodes.Add(new PipeWireNode(id, @class, description));
        id = line[Id.Length..comma].Trim().ToString();
        @class = null;
        description = null;
        return true;
    }

}
