namespace ControlAgentNet.Core.Models;

/// <summary>
/// Thrown when orchestration must pause until a human approves a non-tool workflow step.
/// </summary>
public sealed class NeedsApprovalException : Exception
{
    public string RequestId { get; }

    public NeedsApprovalException(string requestId, string? message = null)
        : base(message ?? "Human approval is required before continuing.")
    {
        RequestId = requestId;
    }
}
