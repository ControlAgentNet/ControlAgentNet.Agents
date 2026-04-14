namespace ControlAgentNet.Core.Models;

public sealed record ToolGuardDecision(
    ToolGuardDecisionKind Kind,
    string? Reason = null,
    string? ApprovalRequestId = null)
{
    public static ToolGuardDecision Allow() => new(ToolGuardDecisionKind.Allow);
    public static ToolGuardDecision Deny(string reason) => new(ToolGuardDecisionKind.Deny, reason);
    public static ToolGuardDecision RequireApproval(string? reason = null, string? approvalRequestId = null)
        => new(ToolGuardDecisionKind.RequireApproval, reason, approvalRequestId);
}
