namespace Soundboword.Linux.PipeWire;

public static class PipeWireNodeReader
{

    private const string Id = "id ";
    private const string Type = "type ";
    private const string InterfaceNode = "PipeWire:Interface:Node/3";
    private const string MediaClass = "media.class";
    private const string Name = "node.name";
    private const string Description = "node.description";

    public static List<PipeWireNode> ReadAudioNodesAsync(StringReader reader)
    {
        var nodes = new List<PipeWireNode>();
        string? id = null, @class = null, name = null, description = null;
        string? line;
        while (!string.IsNullOrWhiteSpace(line = reader.ReadLine()))
        {
            var span = line.AsSpan().Trim();
            if (BeginNewDevice(span, ref id, ref @class, ref name, ref description, nodes) || id == null)
                continue;
            var equals = span.IndexOf('=');
            if (equals == -1)
                continue;
            DetectProperty(span, equals, MediaClass, ref @class);
            DetectProperty(span, equals, Name, ref name);
            DetectProperty(span, equals, Description, ref description);
        }

        return nodes;
    }

    private static void DetectProperty(ReadOnlySpan<char> line, int equals, string propertyName, ref string? value)
    {
        if (line[..equals].Trim().SequenceEqual(propertyName))
            value = line[(equals + 1)..].Trim().Trim('"').ToString();
    }

    private static bool BeginNewDevice(ReadOnlySpan<char> line, ref string? id, ref string? @class, ref string? name, ref string? description, List<PipeWireNode> nodes)
    {
        if (!line.StartsWith(Id))
            return false;
        var comma = line.IndexOf(',');
        var type = line.IndexOf(Type);
        if (comma == -1 || type == -1)
            return false;
        if (id != null && @class != null && name != null && description != null && @class.StartsWith("Audio/"))
            nodes.Add(new PipeWireNode(id, @class, name, description));
        id = line[(type + Type.Length)..].Trim().SequenceEqual(InterfaceNode)
            ? line[Id.Length..comma].Trim().ToString()
            : null;
        @class = null;
        name = null;
        description = null;
        return true;
    }

}
