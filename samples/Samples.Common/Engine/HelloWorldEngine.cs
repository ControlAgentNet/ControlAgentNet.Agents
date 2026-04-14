namespace Samples.Common.Engine;

using ControlAgentNet.Core.Abstractions;
using ControlAgentNet.Core.Models;

public sealed class HelloWorldEngine : IAgentEngine
{
    public Task<AgentEngineResult> RunAsync(AgentContext context, CancellationToken cancellationToken)
        => Task.FromResult(AgentEngineResult.FromText($"Hello world from a custom engine. You said: {context.Message.Text}"));

    public async IAsyncEnumerable<string> StreamAsync(AgentContext context, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        yield return (await RunAsync(context, cancellationToken)).Text;
    }
}
