namespace ControlAgentNet.Core.Descriptors;

public sealed record ToolDescriptor(
    string Id,
    string Name,
    string Description,
    bool DefaultEnabled,
    string Kind,
    string Version,
    CapabilityRiskLevel RiskLevel,
    string SourceAssembly,
    string? Category = null,
    string[]? Tags = null);

