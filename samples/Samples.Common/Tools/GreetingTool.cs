namespace Samples.Common.Tools;

public sealed class GreetingTool
{
    public Task<string> SayGreetingAsync(string name, CancellationToken cancellationToken = default)
        => Task.FromResult($"Hello, {name}! Welcome to ControlAgentNet.");
}
