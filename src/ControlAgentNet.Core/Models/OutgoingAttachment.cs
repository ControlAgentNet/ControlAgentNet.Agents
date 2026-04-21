namespace ControlAgentNet.Core.Models;

public sealed record OutgoingAttachment
{
    public required string Id { get; init; }

    public required string Type { get; init; }

    public string? FileName { get; init; }

    public string? ContentType { get; init; }

    public long? SizeBytes { get; init; }

    public Stream? Content { get; init; }

    public Uri? Url { get; init; }

    public IReadOnlyDictionary<string, string> Metadata { get; init; } = new Dictionary<string, string>();
}
