namespace ControlAgentNet.Core.Models;

public sealed record OutgoingAction
{
    public required string Id { get; init; }

    public required string Type { get; init; }

    public required string Text { get; init; }

    public string? Value { get; init; }

    public IReadOnlyDictionary<string, string> Metadata { get; init; } = new Dictionary<string, string>();
}
