namespace ControlAgentNet.Core.Models;

public sealed record OutgoingMessage
{
    public required string ConversationId { get; init; }

    /// <summary>
    /// The full text of the message if available. 
    /// If ContentStream is used, this may be set ONLY after the stream is fully consumed.
    /// </summary>
    public required string Text { get; init; }

    /// <summary>
    /// When populated, the message is delivered in chunks for a real-time "typing" experience.
    /// </summary>
    public IAsyncEnumerable<string>? ContentStream { get; init; }

    public bool IsStreaming => ContentStream != null;

    /// <summary>
    /// Internal thoughts or tool calls generated during the processing of this message.
    /// </summary>
    public List<AgentThought> Thoughts { get; init; } = new();

    /// <summary>
    /// Telemetry about token usage for this specific message.
    /// </summary>
    public TokenUsage? Usage { get; init; }

    public required string ChannelId { get; init; }

    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    public string? CorrelationId { get; init; }
}
