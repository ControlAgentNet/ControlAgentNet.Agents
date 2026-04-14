using ControlAgentNet.Runtime.Channels;
using ControlAgentNet.Runtime.Tools;

namespace ControlAgentNet.Runtime.Manifest;

public sealed record AgentManifest(
    string? TenantId,
    string AgentId,
    string AgentName,
    string AgentDescription,
    string Instructions,
    IReadOnlyList<ChannelState> Channels,
    IReadOnlyList<ToolState> Tools);
