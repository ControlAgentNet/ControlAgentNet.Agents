using ControlAgentNet.Core.Models;

namespace ControlAgentNet.Core.Abstractions;

/// <summary>
/// Persists human-in-the-loop approval requests and their outcomes.
/// </summary>
public interface IApprovalStore
{
    Task<string> CreateRequestAsync(
        string conversationId,
        string toolName,
        IReadOnlyDictionary<string, object?> arguments,
        CancellationToken cancellationToken = default);

    Task<ApprovalResult?> GetStatusAsync(string requestId, CancellationToken cancellationToken = default);

    Task SetResultAsync(string requestId, bool approved, string? reason, CancellationToken cancellationToken = default);
}
