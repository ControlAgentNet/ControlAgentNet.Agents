namespace ControlAgentNet.Core.Models;

public sealed record IncomingMessage
{
    public required string ConversationId { get; init; }

    public string? TenantId { get; init; }

    public required string UserId { get; init; }

    public required string Text { get; init; }

    public required string ChannelId { get; init; }

    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    public string? CorrelationId { get; init; }
}
