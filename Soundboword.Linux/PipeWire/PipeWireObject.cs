namespace Soundboword.Linux.PipeWire;

public abstract record PipeWireObject(string Id);

public sealed record PipeWireNode(string Id, string Class, string Name, string Description) : PipeWireObject(Id);

public sealed record PipeWirePort(string Id, string Node, int PortId) : PipeWireObject(Id);
