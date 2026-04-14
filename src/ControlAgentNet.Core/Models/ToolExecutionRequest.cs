using ControlAgentNet.Core.Descriptors;

namespace ControlAgentNet.Core.Models;

public sealed record ToolExecutionRequest(
    string ToolId,
    string UserId,
    string ConversationId,
    object? ToolInstance = null,
    Dictionary<string, object?>? Parameters = null,
    ToolDescriptor? Descriptor = null);
