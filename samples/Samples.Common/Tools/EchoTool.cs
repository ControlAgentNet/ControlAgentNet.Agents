namespace Samples.Common.Tools;

public sealed class EchoTool
{
    public Task<string> EchoAsync(string text, CancellationToken cancellationToken = default)
        => Task.FromResult(text);
}
