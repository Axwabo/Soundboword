namespace Soundboword.Linux.PipeWire;

public abstract record PipeWireObject;

public sealed record PipeWireNode(string Id, string Class, string Name, string Description) : PipeWireObject;

public sealed record PipeWirePort(string Id, string Node, string Direction, int PortId) : PipeWireObject;
