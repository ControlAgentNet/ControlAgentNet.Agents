using ControlAgentNet.Core.Descriptors;

namespace ControlAgentNet.Core.Models;

public sealed record ToolExecutionRequest(
    string ToolId,
    string UserId,
    string ConversationId,
    string? TenantId = null,
    string? AgentId = null,
    string? ChannelId = null,
    object? ToolInstance = null,
    Dictionary<string, object?>? Parameters = null,
    ToolDescriptor? Descriptor = null);
