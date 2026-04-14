namespace ControlAgentNet.Core.Descriptors;

public sealed record ChannelDescriptor(
    string Id,
    string Name,
    string Description,
    bool DefaultEnabled,
    ChannelTransportKind Transport,
    string Version,
    string SourceAssembly,
    string? Category = null,
    string[]? Tags = null);

