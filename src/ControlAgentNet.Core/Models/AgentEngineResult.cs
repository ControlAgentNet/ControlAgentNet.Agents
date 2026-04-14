namespace ControlAgentNet.Core.Models;

/// <summary>
/// Represents the full result of an agent engine execution turn,
/// including the response text and optional metadata for observability.
/// </summary>
/// <remarks>
/// This record is designed to be the future return type of <c>IAgentEngine.RunAsync</c>
/// once all providers are updated. Currently it is produced by adapters that wrap
/// the plain <c>string</c> return from engines.
/// </remarks>
public sealed record AgentEngineResult
{
    /// <summary>The generated text response.</summary>
    public required string Text { get; init; }

    /// <summary>
    /// The model identifier used for this turn (e.g., "gpt-4o", "gpt-4-turbo").
    /// Populated when the underlying provider exposes this information.
    /// </summary>
    public string? ModelId { get; init; }

    /// <summary>
    /// Token consumption for this turn.
    /// Populated when the underlying provider exposes usage data.
    /// </summary>
    public TokenUsage? Usage { get; init; }

    /// <summary>
    /// The reason the model stopped generating tokens (e.g., "stop", "length", "tool_calls").
    /// Populated when the underlying provider exposes this information.
    /// </summary>
    public string? FinishReason { get; init; }

    /// <summary>
    /// Convenience factory: wraps a plain text string with no additional metadata.
    /// Used by engines that don't yet return rich metadata.
    /// </summary>
    public static AgentEngineResult FromText(string text) => new() { Text = text };
}
