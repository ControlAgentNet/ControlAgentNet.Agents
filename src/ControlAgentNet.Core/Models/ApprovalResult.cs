namespace ControlAgentNet.Core.Models;

public sealed record ApprovalResult(bool IsReviewed, bool IsApproved, string? Reason);
