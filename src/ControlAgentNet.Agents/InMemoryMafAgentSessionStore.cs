using System.Collections.Concurrent;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;

namespace ControlAgentNet.Agents;

internal sealed class InMemoryMafAgentSessionStore : AgentSessionStore
{
    private readonly ConcurrentDictionary<string, AgentSession> _sessions = new();

    public override ValueTask SaveSessionAsync(AIAgent agent, string conversationId, AgentSession session, CancellationToken cancellationToken)
    {
        _sessions[conversationId] = session;
        return ValueTask.CompletedTask;
    }

    public override ValueTask<AgentSession> GetSessionAsync(AIAgent agent, string conversationId, CancellationToken cancellationToken)
    {
        if (_sessions.TryGetValue(conversationId, out var session))
        {
            return ValueTask.FromResult(session);
        }

        return ValueTask.FromResult<AgentSession>(null!);
    }
}
